using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Testadal.Predicate
{
    public static class PredicateBuilder
    {
        public static IFieldPredicate Equal<T>(Expression<Func<T, object>> expression, object value, bool not = false) where T : class
        {
            return Field(expression, Operator.Equal, value, not);
        }

        public static IFieldPredicate Equal<T>(string propertyName, object value, bool not = false) where T : class
        {
            return Field<T>(propertyName, Operator.Equal, value, not);
        }

        public static IFieldPredicate In<T>(Expression<Func<T, object>> expression, object value, bool not = false) where T : class
        {
            return Field<T>(expression, Operator.In, value, not);
        }

        public static IFieldPredicate In<T>(string propertyName, object value, bool not = false) where T : class
        {
            return Field<T>(propertyName, Operator.In, value, not);
        }

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
