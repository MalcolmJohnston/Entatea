using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Entatea.Model;
using Entatea.Predicate;
using Entatea.Resolvers;

namespace Entatea.SqlBuilder
{
    public abstract class BaseSqlBuilder : ISqlBuilder
    {
        protected readonly ITableNameResolver tableNameResolver = new DefaultTableNameResolver();

        protected readonly IColumnNameResolver columnNameResolver = new DefaultColumnNameResolver();

        public BaseSqlBuilder()
        {
        }

        public BaseSqlBuilder(
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver)
        {
            this.tableNameResolver = tableNameResolver;
            this.columnNameResolver = columnNameResolver;
        }

        private const string OrderByAsc = "ASC";
        private const string OrderByDesc = "DESC";

        public virtual string EncapsulationFormat { get { return "[{0}]"; } }

        public virtual string IsNullFunctionName { get { return "ISNULL"; } }

        public virtual string GetDateFunctionCall { get { return "GETDATE()"; } }

        public virtual string OrderByAscending { get { return OrderByAsc; } }

        public virtual string OrderByDescending { get { return OrderByDesc; } }

        public virtual string Encapsulate(string identifier)
        {
            return string.Format(this.EncapsulationFormat, identifier);
        }

        public virtual string EncapsulateSelect(ClassMap classMap, PropertyMap propertyMap)
        {
            string columnName = this.GetColumnIdentifier(classMap, propertyMap);
            string propertyName = this.Encapsulate(propertyMap.PropertyName);
            
            // if the encapsulated property name and column name are not equal use the AS syntax
            // otherwise just return the encapsulated property name
            return (propertyName != columnName) ? $"{columnName} AS {propertyName}" : propertyName;
        }

        public virtual string CallConcatenate(params object[] parameters)
        {
            return $"CONCAT({string.Join(",", parameters)})";
        }

        public virtual string GetTableIdentifier<T>() where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            return this.GetTableIdentifier(classMap);
        }

        protected virtual string GetTableIdentifier(ClassMap classMap)
        {
            // most dialects do not support schemas so not supported by default

            // if the table name is explict just encapsulate it and return
            if (!string.IsNullOrWhiteSpace(classMap.ExplicitTableName))
            {
                return this.Encapsulate(classMap.ExplicitTableName);
            }

            // otherwise encapsulate the table name returned by the resolver
            return this.Encapsulate(this.tableNameResolver.GetTableName(classMap));
        }

        public virtual string GetColumnIdentifier(ClassMap classMap, PropertyMap propertyMap)
        {
            if (!string.IsNullOrWhiteSpace(propertyMap.ColumnName)) {
                return this.Encapsulate(propertyMap.ColumnName);
            }

            return this.Encapsulate(this.columnNameResolver.GetColumnName(classMap, propertyMap.PropertyName));
        }

        public virtual string GetDeleteWhereSql<T>(IEnumerable<IPredicate> whereConditions) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            return $"DELETE FROM {this.GetTableIdentifier(classMap)} {this.GetWhereClause(whereConditions)}";
        }

        public abstract string GetInsertSql<T>() where T : class;

        public virtual string GetSelectAllSql<T>() where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(string.Join(", ", classMap.SelectProperties.Select(x => this.EncapsulateSelect(classMap, x))));
            sb.Append(" FROM ");
            sb.Append(this.GetTableIdentifier<T>());

            return sb.ToString();
        }

        public virtual string GetSelectCountSql<T>(IEnumerable<IPredicate> whereConditions) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            StringBuilder sb = new StringBuilder($"SELECT COUNT(*) FROM {this.GetTableIdentifier(classMap)}");

            if (whereConditions.Any())
            {
                sb.Append(" ");
                sb.Append(this.GetWhereClause(whereConditions));
            }

            return sb.ToString();
        }

        public virtual string GetSelectNextIdSql<T>() where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            if (!classMap.HasSequentialKey)
            {
                throw new ArgumentException("Cannot generate get next id sql for type that does not have a sequential key.");
            }

            StringBuilder sb = new StringBuilder($"SELECT {this.IsNullFunctionName}(MAX(");
            sb.Append(this.GetColumnIdentifier(classMap, classMap.SequentialKey));
            sb.Append($"), 0) + 1 FROM {this.GetTableIdentifier<T>()}");

            // next id must be constrained by assigned keys and/or partition properties
            // note that they should be added in this order or the parameters WONT match!
            List<PropertyMap> props = new List<PropertyMap>(classMap.AssignedKeys);
            if (classMap.HasSequentialKey && classMap.SequentialKey.KeyType == KeyType.SequentialPartition)
            {
                props.Add(classMap.SequentialKey);
            }

            if (props.Any())
            {
                sb.Append($" {this.GetByPropertiesWhereClause(classMap, props)}");
            }
            
            return sb.ToString();
        }

        public virtual string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions) where T : class
        {
            StringBuilder sb = new StringBuilder(this.GetSelectAllSql<T>());
            sb.Append(" ");
            sb.Append(this.GetWhereClause(whereConditions));

            return sb.ToString();
        }

        public abstract string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions, object sortOrders, int firstRow, int lastRow) where T : class;

        public virtual string GetUpdateSql<T>(object properties) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            if (properties == null)
            {
                throw new ArgumentException("Please provide one or more properties to update.");
            }

            // build list of columns to update
            List<PropertyMap> updateMaps = new List<PropertyMap>();
            if (properties is IDictionary<string, object>)
            {
                var propertyDict = properties as IDictionary<string, object>;
                foreach (string key in propertyDict.Keys)
                {
                    PropertyMap propertyMap = classMap.UpdateableProperties.SingleOrDefault(x => x.PropertyName == key);
                    if (propertyMap != null)
                    {
                        updateMaps.Add(propertyMap);
                    }
                }
            }
            else
            {
                PropertyInfo[] propertyInfos = properties.GetType().GetProperties();
                foreach (PropertyInfo pi in propertyInfos)
                {
                    PropertyMap propertyMap = classMap.UpdateableProperties.SingleOrDefault(x => x.PropertyName == pi.Name);
                    if (propertyMap != null)
                    {
                        updateMaps.Add(propertyMap);
                    }
                }
            }            

            // check we have properties to update
            if (updateMaps.Count == 0)
            {
                throw new ArgumentException("Please provide one or more updateable properties.");
            }

            // build the update sql
            StringBuilder sb = new StringBuilder($"UPDATE {this.GetTableIdentifier(classMap)} SET ");

            // add all update properties to SET clause
            for (int i = 0; i < updateMaps.Count; i++)
            {
                sb.Append($"{this.GetColumnIdentifier(classMap, updateMaps[i])} = @{updateMaps[i].PropertyName}, ");
            }

            // remove trailing separator (either from update maps or date stamps)
            sb.Remove(sb.Length - 2, 2);

            // add where clause
            sb.Append($" {this.GetByIdWhereClause(classMap)}");

            return sb.ToString();
        }
    }
}
