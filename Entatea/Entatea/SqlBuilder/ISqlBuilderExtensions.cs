using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entatea.Model;
using Entatea.Predicate;

namespace Entatea.SqlBuilder
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

        public static string GetDeleteSql<T>(this ISqlBuilder sqlBuilder) where T : class
        {
            return $"DELETE FROM {sqlBuilder.GetTableIdentifier<T>()}";
        }

        public static string GetWhereClause(this ISqlBuilder sqlBuilder, IEnumerable<IPredicate> predicates)
        {
            int count = predicates.Count();
            if (predicates == null || count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder("WHERE ");
            int parameterIndex = 0;
            for (int i = 0; i < count; i++)
            {
                sb.Append(predicates.ElementAt(i).GetSql(sqlBuilder, parameterIndex, out int parameterCount));
                parameterIndex += parameterCount;

                // exclude AND on last property
                if ((i + 1) < count)
                {
                    sb.Append(" AND ");
                }
            }

            return sb.ToString();
        }

        public static string GetByIdWhereClause(this ISqlBuilder sqlBuilder, ClassMap classMap)
        {
            // as this is a utility method then we need to add the default properties here
            // for all other where clause building assume these have been passed in
            List<PropertyMap> props = new List<PropertyMap>(classMap.AllKeys);
            if (classMap.DiscriminatorProperties.Any())
            {
                props.AddRange(classMap.DiscriminatorProperties);
            }
            if (classMap.SoftDeleteProperty != null)
            {
                props.Add(classMap.SoftDeleteProperty);
            }

            return GetByPropertiesWhereClause(sqlBuilder, props);
        }

        public static string GetByPropertiesWhereClause(this ISqlBuilder sqlBuilder, IEnumerable<PropertyMap> properties)
        {
            StringBuilder sb = new StringBuilder("WHERE ");

            int count = properties.Count();
            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                PropertyMap pm = properties.ElementAt(i);

                int paramIdx = i + offset;

                if (pm.KeyType == KeyType.SequentialPartition)
                {
                    if (pm.PartitionFromValue != null && pm.PartitionToValue != null)
                    {
                        sb.Append($"{sqlBuilder.GetColumnIdentifier(pm)} BETWEEN @p{paramIdx} AND @p{paramIdx + 1}");
                        offset++;
                    }
                    else if (pm.PartitionFromValue != null)
                    {
                        sb.Append($"{sqlBuilder.GetColumnIdentifier(pm)} >= @p{paramIdx}");
                    }
                    else if (pm.PartitionToValue != null)
                    {
                        sb.Append($"{sqlBuilder.GetColumnIdentifier(pm)} <= @p{paramIdx}");
                    }
                }
                else
                {
                    sb.Append($"{sqlBuilder.GetColumnIdentifier(pm)} = @p{paramIdx}");
                }

                // exclude AND on last property
                if ((i + 1) < count)
                {
                    sb.Append(" AND ");
                }
            }

            return sb.ToString();
        }

        public static string GetOrderByClause<T>(this ISqlBuilder sqlBuilder, object sortOrders) where T : class
        {
            // coalesce the dictionary
            ClassMap classMap = ClassMapper.GetClassMap<T>();
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

                orderBySb.Append($"{sqlBuilder.GetColumnIdentifier(pm)} {sqlBuilder.GetSortOrder(order)}");

                if (i != sortOrderDict.Count - 1)
                {
                    orderBySb.Append(", ");
                }
            }

            return orderBySb.ToString();
        }
    }
}
