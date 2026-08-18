using System.Reflection;

namespace Entatea.InMemory
{
    internal static class ShallowCopy
    {
        private static readonly MethodInfo memberwiseClone = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        public static T Of<T>(T source) where T : class
        {
            return source == null ? null : (T)memberwiseClone.Invoke(source, null);
        }
    }
}
