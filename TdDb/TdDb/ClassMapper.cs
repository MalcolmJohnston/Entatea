using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using System.Text;
using TdDb.Model;

namespace TdDb
{
    public static class ClassMapper
    {
        private static readonly ConcurrentDictionary<string, ClassMap> cache = new ConcurrentDictionary<string, ClassMap>();
        public static ClassMap GetClassMap<T>()
        {
            Type type = typeof(T);
            if (!cache.ContainsKey(type.FullName))
            {
                cache[type.FullName] = new ClassMap(type);
            }

            return cache[type.FullName];
        }
    }
}
