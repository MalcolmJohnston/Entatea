using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Entatea.Predicate
{
    public static partial class PredicateBuilder
    {
        public static IFieldPredicate Equal<T>(Expression<Func<T, object>> expression, object value) where T : class
        {
            return Field(expression, Operator.Equal, value);
        }

        public static IFieldPredicate Equal<T>(string propertyName, object value) where T : class
        {
            return Field<T>(propertyName, Operator.Equal, value);
        }
    }
}
