﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entatea.Model;
using Entatea.Predicate;
using Entatea.Resolvers;
using Entatea.SqlBuilder;

namespace Entatea.SqlServer
{
    public class TSqlBuilder : BaseSqlBuilder, ISqlBuilder
    {
        private const string DefaultSchema = "dbo";

        private readonly ISchemaResolver schemaResolver;

        public TSqlBuilder() : base()
        {
            this.schemaResolver = new DefaultSchemaResolver(DefaultSchema);
        }

        public TSqlBuilder(
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
                tableNameResolver,
                columnNameResolver)
        {
            this.schemaResolver = new DefaultSchemaResolver(DefaultSchema);
        }

        public TSqlBuilder(
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver,
            string defaultSchema) : base(
                tableNameResolver,
                columnNameResolver)
        {
            this.schemaResolver = new DefaultSchemaResolver(defaultSchema);
        }

        public TSqlBuilder(
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver,
            ISchemaResolver schemaResolver) : base(
                tableNameResolver,
                columnNameResolver)
        {
            this.schemaResolver = schemaResolver;
        }

        protected override string GetTableIdentifier(ClassMap classMap)
        {
            string tableName = classMap.ExplicitTableName;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = this.tableNameResolver.GetTableName(classMap);
            }

            string schemaName = this.schemaResolver.GetSchema(classMap);
            
            return $"{this.Encapsulate(schemaName)}.{this.Encapsulate(tableName)}";
        }

        public override string GetInsertSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // setup the insert with table name
            StringBuilder sb = new StringBuilder($"INSERT INTO {this.GetTableIdentifier(classMap)} (");

            // add the columns we are inserting
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => this.GetColumnIdentifier(classMap, x))));
            sb.Append(")");

            // add identity column outputs
            if (classMap.HasIdentityKey)
            {
                sb.Append($" OUTPUT inserted.{this.EncapsulateSelect(classMap, classMap.IdentityKey)}");
            }

            // add parameterised values
            sb.Append(" VALUES (");
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => $"@{x.PropertyName}")));
            sb.Append(")");

            return sb.ToString();
        }

        public override string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions, object sortOrders, int firstRow, int lastRow)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // build the WHERE clause (if any specified)
            string where = this.GetWhereClause(whereConditions);

            // build the ORDER BY clause (always required and will default to primary key columns ascending, if none specified)
            string orderBy = this.GetOrderByClause<T>(sortOrders);

            // build paging sql
            return $@"SELECT {string.Join(", ", classMap.SelectProperties.Select(x => this.EncapsulateSelect(classMap, x)))}
                        FROM (SELECT ROW_NUMBER() OVER ({orderBy}) AS RowNo, *
                                FROM {this.GetTableIdentifier(classMap)}
                                {where}) tmp
                        WHERE tmp.RowNo BETWEEN {firstRow} AND {lastRow};";
        }
    }
}
