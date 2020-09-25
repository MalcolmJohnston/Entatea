using System;
using System.Collections.Concurrent;

using Entatea.Model;
using Entatea.Resolvers;

namespace Entatea
{
    public static class ClassMapper
    {
        private static readonly ConcurrentDictionary<string, ClassMap> cache = new ConcurrentDictionary<string, ClassMap>();
        public static ClassMap GetClassMap<T>() where T : class
        {
            Type type = typeof(T);
            return GetClassMap(type);
        }

        public static ClassMap GetClassMap(Type type)
        {
            if (!cache.ContainsKey(type.FullName))
            {
                cache[type.FullName] = new ClassMap(type);
            }

            return cache[type.FullName];
        }
    }
}
