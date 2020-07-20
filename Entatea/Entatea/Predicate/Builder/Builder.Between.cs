using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Entatea.Predicate
{
    public static partial class PredicateBuilder
    {
        private static IBetweenPredicate Between<T>(string propertyName, object value1, object value2, bool not = false) where T : class
        {
            return new BetweenPredicate<T>
            {
                PropertyName = propertyName,
                Operator = Operator.Between,
                Value = value1,
                Value2 = value2,
                Not = not
            };
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, byte>> expression, byte value1, byte value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, sbyte>> expression, sbyte value1, sbyte value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, short>> expression, short value1, short value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, ushort>> expression, ushort value1, ushort value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, int>> expression, int value1, int value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, uint>> expression, uint value1, uint value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, long>> expression, long value1, long value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, ulong>> expression, ulong value1, ulong value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, float>> expression, float value1, float value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, double>> expression, double value1, double value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, decimal>> expression, decimal value1, decimal value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, DateTime>> expression, DateTime value1, DateTime value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, DateTimeOffset>> expression, DateTimeOffset value1, DateTimeOffset value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, byte?>> expression, byte? value1, byte? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, sbyte?>> expression, sbyte? value1, sbyte? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, short?>> expression, short? value1, short? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, ushort?>> expression, ushort? value1, ushort? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, int?>> expression, int? value1, int? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, uint?>> expression, uint? value1, uint? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, long?>> expression, long? value1, long? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, ulong?>> expression, ulong? value1, ulong? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, float?>> expression, float? value1, float? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, double?>> expression, double? value1, double? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, decimal?>> expression, decimal? value1, decimal? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, DateTime?>> expression, DateTime? value1, DateTime? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }

        public static IBetweenPredicate Between<T>(Expression<Func<T, DateTimeOffset?>> expression, DateTimeOffset? value1, DateTimeOffset? value2) where T : class
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Between<T>(propertyInfo.Name, value1, value2);
        }
    }
}
