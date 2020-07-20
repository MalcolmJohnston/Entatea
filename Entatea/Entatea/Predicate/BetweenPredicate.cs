using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Entatea.Model;
using Entatea.SqlBuilder;

namespace Entatea.Predicate
{
    public class BetweenPredicate<T> : ComparePredicate, IBetweenPredicate
        where T : class
    {
        public string PropertyName { get; set; }

        public object Value { get; set; }

        public object Value2 { get; set; }

        public string GetSql(ISqlBuilder sqlBuilder, int parameterIndex, out int parameterCount)
        {
            parameterCount = 2;
            return $"{sqlBuilder.GetColumnName<T>(PropertyName)} {GetOperatorString()} @p{parameterIndex} AND @p{parameterIndex+1}";
        }

        public IEnumerable<KeyValuePair<string, object>> GetParameters(int parameterIndex, out int parameterCount)
        {
            parameterCount = 2;
            return new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>($"@p{parameterIndex}", this.Value),
                new KeyValuePair<string, object>($"@p{parameterIndex+1}", this.Value2)
            };
        }
    }
}
