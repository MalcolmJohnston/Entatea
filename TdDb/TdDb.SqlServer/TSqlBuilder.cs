using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using TdDb.Model;
using TdDb.Predicate;
using TdDb.SqlBuilder;

namespace TdDb.SqlServer
{
    public class TSqlBuilder : ISqlBuilder
    {
        private const string OrderByAsc = "ASC";
        private const string OrderByDesc = "DESC";

        public string EncapsulationFormat { get { return "[{0}]"; } }

        public string OrderByAscending { get { return OrderByAsc; } }

        public string OrderByDescending { get { return OrderByDesc; } }

        public string GetColumn(PropertyMap propertyMap)
        {
            if (propertyMap.PropertyName != propertyMap.ColumnName)
            {
                return $"{propertyMap.ColumnName} AS {propertyMap.PropertyName}";
            }

            return propertyMap.ColumnName;
        }

        public string GetTableIdentifier<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            return this.GetTableIdentifier(classMap);
        }

        private string GetTableIdentifier(ClassMap classMap)
        {
            return !string.IsNullOrWhiteSpace(classMap.Schema) ? $"[{classMap.Schema}].[{classMap.TableName}]" : $"[{classMap.TableName}]";
        }

        public string GetDeleteByIdSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            return $"DELETE FROM {this.GetTableIdentifier(classMap)} {this.GetByIdWhereClause(classMap)}";
        }

        public string GetDeleteWhereSql<T>(object whereConditions)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            return $"DELETE FROM {this.GetTableIdentifier(classMap)} {this.GetWhereClause(classMap, whereConditions)}";
        }

        public string GetInsertSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // setup the insert with table name
            StringBuilder sb = new StringBuilder($"INSERT INTO {this.GetTableIdentifier(classMap)} (");

            // add the columns we are inserting
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => x.ColumnName)));
            sb.Append(")");

            // add identity column outputs
            if (classMap.HasIdentityKey)
            {
                sb.Append($" OUTPUT inserted.{classMap.IdentityKey.ColumnName}");
            }

            // add parameterised values
            sb.Append(" VALUES (");
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => $"@{x.PropertyName}")));
            sb.Append(")");

            return sb.ToString();
        }

        public string GetSelectAllSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(string.Join(", ", classMap.SelectProperties.Select(x => this.GetColumn(x))));
            sb.Append(" FROM ");
            sb.Append(this.GetTableIdentifier<T>());

            return sb.ToString();
        }

        public string GetSelectByIdSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetSelectAllSql<T>());
            sb.Append(" ");
            sb.Append(this.GetByIdWhereClause(classMap));

            return sb.ToString();
        }

        public string GetSelectCountSql<T>(object whereConditions)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            StringBuilder sb = new StringBuilder($"SELECT COUNT(*) FROM {this.GetTableIdentifier(classMap)}");

            IDictionary<string, object> whereConditionsDict = classMap.CoalesceToDictionary(whereConditions);
            if (whereConditionsDict.Any())
            {
                sb.Append(" ");
                sb.Append(this.GetWhereClause(classMap, whereConditions));
            }

            return sb.ToString();
        }

        public string GetSelectNextIdSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            if (!classMap.HasSequentialKey)
            {
                throw new ArgumentException("Cannot generate get next id sql for type that does not have a sequential key.");
            }

            StringBuilder sb = new StringBuilder("SELECT ISNULL(MAX(");
            sb.Append(classMap.SequentialKey.ColumnName);
            sb.Append($"), 0) + 1 FROM {this.GetTableIdentifier<T>()}");

            if (classMap.HasAssignedKeys)
            {
                sb.Append(
                    this.GetWhereClause(
                        classMap.AssignedKeys.Select(x => new Equal(x.ColumnName, x.PropertyName)).ToList<IPredicate>()));
            }

            return sb.ToString();
        }

        public string GetSelectWhereSql<T>(object whereConditions)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            classMap.ValidateWhereProperties(whereConditions);

            StringBuilder sb = new StringBuilder(this.GetSelectAllSql<T>());
            sb.Append(" ");
            sb.Append(this.GetWhereClause(classMap, whereConditions));

            return sb.ToString();
        }

        public string GetSelectWhereSql<T>(object whereConditions, object sortOrders, int firstRow, int lastRow)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // build the WHERE clause (if any specified)
            IDictionary<string, object> whereConditionsDict = classMap.CoalesceToDictionary(whereConditions);
            string where = whereConditionsDict.Any() ? this.GetWhereClause(classMap, whereConditions) : string.Empty;

            // build the ORDER BY clause (always required and will default to primary key columns ascending, if none specified)
            string orderBy = this.GetOrderByClause(classMap, sortOrders);

            // build paging sql
            return $@"SELECT {string.Join(", ", classMap.SelectProperties.Select(x => this.GetColumn(x)))}
                        FROM (SELECT ROW_NUMBER() OVER ({orderBy}) AS RowNo, *
                                FROM {this.GetTableIdentifier(classMap)}
                                {where}) tmp
                        WHERE tmp.RowNo BETWEEN {firstRow} AND {lastRow};";
        }

        public string GetUpdateSql<T>(object properties)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            if (properties == null)
            {
                throw new ArgumentException("Please provide one or more properties to update.");
            }

            // build list of columns to update
            List<PropertyMap> updateMaps = new List<PropertyMap>();
            PropertyInfo[] propertyInfos = properties.GetType().GetProperties();

            // loop through our updateable properties and add them as required
            foreach (PropertyInfo pi in propertyInfos)
            {
                PropertyMap propertyMap = classMap.UpdateableProperties.Where(x => x.PropertyName == pi.Name).SingleOrDefault();
                if (propertyMap != null)
                {
                    updateMaps.Add(propertyMap);
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
                sb.Append($"{updateMaps[i].ColumnName} = @{updateMaps[i].PropertyName}, ");
            }

            // deal with date stamp properties
            int dateStampCount = classMap.DateStampProperties.Where(x => !x.IsReadOnly).Count();
            if (dateStampCount > 0)
            {
                // add any Date Stamp properties to the SET clause
                foreach (PropertyMap pm in classMap.DateStampProperties.Where(x => !x.IsReadOnly))
                {
                    sb.Append($"{pm.ColumnName} = GETDATE(), ");
                }
            }

            // remove trailing separator (either from update maps or date stamps)
            sb.Remove(sb.Length - 2, 2);

            // add where clause
            sb.Append($" {this.GetByIdWhereClause(classMap)}");

            return sb.ToString();
        }
    
        public string PredicateToSql(IPredicate predicate)
        {
            if (predicate.Operator == Operator.Equal)
            {
                return $"{predicate.ColumnName} = @{predicate.PropertyName}";
            } 
            else if (predicate.Operator == Operator.In)
            {
                return $"{predicate.ColumnName} IN @{predicate.PropertyName}";
            }

            return string.Empty;
        }
    }
}
