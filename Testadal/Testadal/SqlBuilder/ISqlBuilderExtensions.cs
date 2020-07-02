using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Testadal.Data;
using Testadal.Model;
using Testadal.Predicate;

namespace Testadal.SqlBuilder
{
    /// <summary>
    /// Extension methods for the ISqlBuilder interface.
    /// </summary>
    public static class ISqlBuilderExtensions
    {
        public static string GetSortOrder(this ISqlBuilder sqlBuilder, SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Ascending ? sqlBuilder.OrderByAscending : sqlBuilder.OrderByDescending;
        }

        public static string GetDeleteSql<T>(this ISqlBuilder sqlBuilder)
        {
            return $"DELETE FROM {sqlBuilder.GetTableIdentifier<T>()}";
        }

        public static string GetWhereClause(this ISqlBuilder sqlBuilder, IList<IPredicate> operations)
        {
            if (operations == null || operations.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder("WHERE ");
            for (int i = 0; i < operations.Count; i++)
            {
                sb.Append(sqlBuilder.PredicateToSql(operations[i]));

                // exclude AND on last property
                if ((i + 1) < operations.Count)
                {
                    sb.Append(" AND ");
                }
            }

            return sb.ToString();
        }

        public static string GetWhereClause(this ISqlBuilder sqlBuilder, ClassMap classMap, object whereConditions)
        {
            if (whereConditions == null)
            {
                throw new ArgumentException("Please specify some conditions to search by.");
            }

            // build a list of properties for the WHERE clause
            // this will error if no properties are found that match our Type
            IList<IPredicate> properties = classMap.ValidateWhereProperties(whereConditions);

            // return the WHERE clause
            return sqlBuilder.GetWhereClause(properties);
        }

        public static string GetByIdWhereClause(this ISqlBuilder sqlBuilder, ClassMap classMap)
        {
            return sqlBuilder.GetWhereClause(classMap.AllKeys.Select(x => new Equal(x.ColumnName, x.PropertyName))
                                                             .ToList<IPredicate>());
        }

        public static string GetOrderByClause(this ISqlBuilder sqlBuilder, ClassMap classMap, object sortOrders)
        {
            // coalesce the dictionary
            IDictionary<string, SortOrder> sortOrderDict = classMap.CoalesceSortOrderDictionary(sortOrders);

            // validate / return
            StringBuilder orderBySb = new StringBuilder("ORDER BY ");
            for (int i = 0; i < sortOrderDict.Count; i++)
            {
                string propertyName = sortOrderDict.Keys.ElementAt(i);
                SortOrder order = sortOrderDict[propertyName];

                if (classMap.AllProperties.TryGetValue(propertyName, out PropertyMap pm) == false)
                {
                    throw new ArgumentException($"Failed to find property {propertyName} on {classMap.Name}");
                }

                orderBySb.Append($"{pm.ColumnName} {sqlBuilder.GetSortOrder(order)}");

                if (i != sortOrderDict.Count - 1)
                {
                    orderBySb.Append(", ");
                }
            }

            return orderBySb.ToString();
        }
    }
}
