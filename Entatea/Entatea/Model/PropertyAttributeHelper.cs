using System.Reflection;
using System.Linq;

namespace Entatea.Model
{
    internal static class PropertyAttributeHelper
    {
        internal static bool IsNotMapped(PropertyInfo pi) { return HasAttribute(pi, "NotMappedAttribute"); }

        internal static bool IsKey(PropertyInfo pi) { return HasAttribute(pi, "KeyAttribute"); }

        internal static bool IsRequired(PropertyInfo pi) { return HasAttribute(pi, "RequiredAttribute"); }

        internal static bool IsDateStamp(PropertyInfo pi) { return HasAttribute(pi, "DateStampAttribute"); }

        internal static KeyType GetKeyType(PropertyInfo pi)
        {
            dynamic attr = GetAttribute(pi, "KeyTypeAttribute");
            if (attr != null)
            {
                return attr.KeyType;
            }

            return KeyType.NotAKey;
        }

        internal static dynamic GetSoftDelete(PropertyInfo pi)
        {
            dynamic attr = GetAttribute(pi, "SoftDeleteAttribute");
            if (attr != null)
            {
                return attr;
            }

            return null;
        }

        internal static dynamic GetDiscriminator(PropertyInfo pi)
        {
            dynamic attr = GetAttribute(pi, "DiscriminatorAttribute");
            if (attr != null)
            {
                return attr;
            }

            return null;
        }

        internal static bool IsEditable(PropertyInfo pi)
        {
            dynamic attr = GetAttribute(pi, "EditableAttribute");
            if (attr != null)
            {
                return attr.AllowEdit;
            }

            return true;
        }

        internal static bool IsReadOnly(PropertyInfo pi)
        {
            dynamic attr = GetAttribute(pi, "ReadOnlyAttribute");
            if (attr != null)
            {
                return attr.IsReadOnly;
            }

            return false;
        }

        internal static dynamic GetColumnAttribute(PropertyInfo pi)
        {
            return GetAttribute(pi, "ColumnAttribute");
        }

        private static bool HasAttribute(PropertyInfo pi, string attributeName)
        {
            return pi.GetCustomAttributes(false).Any(x => x.GetType().Name == attributeName);
        }

        private static dynamic GetAttribute(PropertyInfo pi, string attributeName)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                return attributes.FirstOrDefault(x => x.GetType().Name == attributeName);
            }

            return null;
        }
    }
}
