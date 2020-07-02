using System.Collections.Generic;
using System.Linq;
using System.Text;

using Testadal.Model;
using Testadal.SqlBuilder;

namespace Testadal.SqlServer
{
    public class TSqlBuilder : BaseSqlBuilder, ISqlBuilder
    {
        protected override string GetTableIdentifier(ClassMap classMap)
        {
            string tableName = string.Format(this.EncapsulationFormat, classMap.TableName.ToLower());
            if (string.IsNullOrWhiteSpace(classMap.Schema))
            {
                return tableName;
            }
            else
            {
                string schemaName = string.Format(this.EncapsulationFormat, classMap.Schema.ToLower());
                return $"{schemaName}.{tableName}";
            }
        }

        public override string GetInsertSql<T>()
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // setup the insert with table name
            StringBuilder sb = new StringBuilder($"INSERT INTO {this.GetTableIdentifier(classMap)} (");

            // add the columns we are inserting
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => this.Encapsulate(x.ColumnName))));
            sb.Append(")");

            // add identity column outputs
            if (classMap.HasIdentityKey)
            {
                sb.Append($" OUTPUT inserted.{this.Encapsulate(classMap.IdentityKey.ColumnName)}");
            }

            // add parameterised values
            sb.Append(" VALUES (");
            sb.Append(string.Join(", ", classMap.InsertableProperties.Select(x => $"@{x.PropertyName}")));
            sb.Append(")");

            return sb.ToString();
        }

        public override string GetSelectWhereSql<T>(object whereConditions, object sortOrders, int firstRow, int lastRow)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // build the WHERE clause (if any specified)
            IDictionary<string, object> whereConditionsDict = classMap.CoalesceToDictionary(whereConditions);
            string where = whereConditionsDict.Any() ? this.GetWhereClause(classMap, whereConditions) : string.Empty;

            // build the ORDER BY clause (always required and will default to primary key columns ascending, if none specified)
            string orderBy = this.GetOrderByClause(classMap, sortOrders);

            // build paging sql
            return $@"SELECT {string.Join(", ", classMap.SelectProperties.Select(x => this.EncapsulateSelect(x)))}
                        FROM (SELECT ROW_NUMBER() OVER ({orderBy}) AS RowNo, *
                                FROM {this.GetTableIdentifier(classMap)}
                                {where}) tmp
                        WHERE tmp.RowNo BETWEEN {firstRow} AND {lastRow};";
        }
    }
}
