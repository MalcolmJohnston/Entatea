using System;
using System.Collections;
using System.Linq.Expressions;

namespace Testadal.Predicate
{
    public static partial class PredicateBuilder
    {
        public static IFieldPredicate NotIn<T>(Expression<Func<T, object>> expression, IEnumerable value) where T : class
        {
            return Field<T>(expression, Operator.In, value, true);
        }

        public static IFieldPredicate NotIn<T>(string propertyName, IEnumerable value) where T : class
        {
            return Field<T>(propertyName, Operator.In, value, true);
        }
    }
}
