using System;
using System.Collections;
using System.Linq.Expressions;

namespace Entatea.Predicate
{
    public static partial class PredicateBuilder
    {
        public static IFieldPredicate In<T>(Expression<Func<T, object>> expression, IEnumerable value) where T : class
        {
            return Field<T>(expression, Operator.In, value);
        }

        public static IFieldPredicate In<T>(string propertyName, IEnumerable value) where T : class
        {
            return Field<T>(propertyName, Operator.In, value);
        }
    }
}
