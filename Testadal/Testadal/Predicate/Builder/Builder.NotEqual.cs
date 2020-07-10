using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Testadal.Predicate
{
    public static partial class PredicateBuilder
    {
        public static IFieldPredicate NotEqual<T>(Expression<Func<T, object>> expression, object value) where T : class
        {
            return Field(expression, Operator.Equal, value, true);
        }

        public static IFieldPredicate NotEqual<T>(string propertyName, object value) where T : class
        {
            return Field<T>(propertyName, Operator.Equal, value, true);
        }
    }
}
