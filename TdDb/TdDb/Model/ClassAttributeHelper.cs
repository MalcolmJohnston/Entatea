using System;
using System.Linq;

namespace TdDb.Model
{
    internal class ClassAttributeHelper
    {
        internal static dynamic GetTableAttribute(Type type)
        {
            return GetAttribute(type, "TableAttribute");
        }

        private static dynamic GetAttribute(Type type, string attributeName)
        {
            var attributes = type.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                return attributes.FirstOrDefault(x => x.GetType().Name == attributeName);
            }

            return null;
        }
    }
}
