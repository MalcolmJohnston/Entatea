using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entatea.Model;
using Entatea.Predicate;
using Entatea.Resolvers;
using Entatea.SqlBuilder;

namespace Entatea.MySql
{
    public class MySqlBuilder : BaseSqlBuilder, ISqlBuilder
    {
        public MySqlBuilder() { }

        public MySqlBuilder(
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
                tableNameResolver,
                columnNameResolver)
        {
        }

        public override string EncapsulationFormat { get { return "`{0}`"; } }

        public override string IsNullFunctionName { get { return "IFNULL"; } }

        public override string GetDateFunctionCall { get { return "NOW()"; } }

        public override string GetInsertSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // setup the insert with table name
            StringBuilder sb = new StringBuilder($"INSERT INTO {this.GetTableIdentifier(classMap)} (");

            // add the columns we are inserting
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => this.GetColumnIdentifier(x))));
            sb.Append(")");

            // add parameterised values
            sb.Append(" VALUES (");
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => $"@{x.PropertyName}")));
            sb.Append(");");

            // add identity column outputs
            if (classMap.HasIdentityKey)
            {
                sb.Append($" SELECT LAST_INSERT_ID() AS {classMap.IdentityKey.PropertyName};");
            }

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
            int offset = firstRow - 1;
            int count = lastRow - offset;
            return $@"SELECT {string.Join(", ", classMap.SelectProperties.Select(x => this.EncapsulateSelect(x)))}
                        FROM {this.GetTableIdentifier(classMap)}
                        {where} {orderBy}
                        LIMIT {offset}, {count};";
        }
    }
}
