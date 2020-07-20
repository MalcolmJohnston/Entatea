using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Entatea.Predicate
{
    public static partial class PredicateBuilder
    {
        private static IFieldPredicate Field<T>(Expression<Func<T, object>> expression, Operator op, object value, bool not = false) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, op, value, not);
        }

        private static IFieldPredicate Field<T>(string propertyName, Operator op, object value, bool not = false) where T : class
        {
            return new FieldPredicate<T>
            {
                PropertyName = propertyName,
                Operator = op,
                Value = value,
                Not = not
            };
        }
    }
}
