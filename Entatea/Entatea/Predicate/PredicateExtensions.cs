using Entatea.Model;
using Entatea.SqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Entatea.Predicate
{
    public static class PredicateExtensions
    {
        public static string GetColumnName<T>(this ISqlBuilder sqlBuilder, string propertyName) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            if (classMap == null)
            {
                throw new NullReferenceException($"Map was not found for {typeof(T)}");
            }

            PropertyMap propertyMap = classMap.AllProperties[propertyName];
            if (propertyMap == null)
            {
                throw new NullReferenceException($"{propertyName} was not found for {typeof(T)}");
            }

            return sqlBuilder.GetColumnIdentifier(classMap, propertyMap);
        }

        public static IDictionary<string, object> GetParameters(
            this IEnumerable<IPredicate> predicates,
            IDictionary<string, object> nonIndexedParameters = null)
        {
            if (nonIndexedParameters == null)
            {
                nonIndexedParameters = new Dictionary<string, object>();
            }

            int parameterIndex = 0;
            foreach (IPredicate predicate in predicates)
            {
                var kvps = predicate.GetParameters(parameterIndex, out int parameterCount).ToList();

                if (predicate.IsInOperator() && kvps.Where(x => x.Value is IEnumerable && !(x.Value is String)).Sum(x => ((IEnumerable)x.Value).Cast<object>().Count()) > 1)
                {
                    var nonNullValues = kvps.Select(kvp => new KeyValuePair<string, object>(
                                kvp.Key, (kvp.Value as IEnumerable)?
                                    .Cast<object>().Where(x => x != null).ToList()
                            )).ToList();

                    nonNullValues.ForEach(x => nonIndexedParameters.Add(x.Key, x.Value));
                } 
                else
                {
                    kvps.ForEach(x => nonIndexedParameters.Add(x.Key, x.Value));
                }

                parameterIndex += parameterCount;
            }

            return nonIndexedParameters;
        }
    }
}
