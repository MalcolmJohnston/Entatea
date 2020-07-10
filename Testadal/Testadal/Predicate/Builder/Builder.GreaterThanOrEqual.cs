using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Testadal.Predicate
{
    public static partial class PredicateBuilder
    {
        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, byte>> expression, byte value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, sbyte>> expression, sbyte value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, short>> expression, short value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, ushort>> expression, ushort value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, int>> expression, int value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, uint>> expression, uint value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, long>> expression, long value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, ulong>> expression, ulong value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, float>> expression, float value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, double>> expression, double value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, decimal>> expression, decimal value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, DateTime>> expression, DateTime value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }

        public static IFieldPredicate GreaterThanOrEqual<T>(Expression<Func<T, DateTimeOffset>> expression, DateTimeOffset value) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Field<T>(propertyInfo.Name, Operator.GreaterThanOrEqual, value);
        }
    }
}
