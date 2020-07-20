using System;
using System.Collections.Concurrent;

using Entatea.Model;

namespace Entatea.Cache
{
    public class SqlCache : ISqlCache
    {
        private readonly ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        public string GetSelectAllSql<T>()
        {
            return GetCacheSql<T>(SqlCacheKey.SelectAll);
        }

        public string GetSelectByIdSql<T>()
        {
            return GetCacheSql<T>(SqlCacheKey.SelectById);
        }

        public string GetSelectCountSql<T>()
        {
            return GetCacheSql<T>(SqlCacheKey.SelectCount);
        }
        public string GetSelectNextIdSql<T>()
        {
            return GetCacheSql<T>(SqlCacheKey.SelectNextId);
        }

        public string GetInsertSql<T>()
        {
            return GetCacheSql<T>(SqlCacheKey.Insert);
        }
        public string GetDeleteByIdSql<T>()
        {
            return GetCacheSql<T>(SqlCacheKey.DeleteById);
        }

        public void SetSelectAllSql<T>(string sql)
        {
            this.SetCacheSql<T>(SqlCacheKey.SelectAll, sql);
        }
        public void SetSelectByIdSql<T>(string sql)
        {
            this.SetCacheSql<T>(SqlCacheKey.SelectById, sql);
        }

        public void SetSelectCountSql<T>(string sql)
        {
            this.SetCacheSql<T>(SqlCacheKey.SelectCount, sql);
        }

        public void SetSelectNextIdSql<T>(string sql)
        {
            this.SetCacheSql<T>(SqlCacheKey.SelectNextId, sql);
        }
        public void SetInsertSql<T>(string sql)
        {
            this.SetCacheSql<T>(SqlCacheKey.Insert, sql);
        }

        public void SetDeleteByIdSql<T>(string sql)
        {
            this.SetCacheSql<T>(SqlCacheKey.DeleteById, sql);
        }

        private string GetCacheSql<T>(SqlCacheKey sqlType)
        {
            string key = GetCacheKey<T>(sqlType);
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            return string.Empty;
        }

        private void SetCacheSql<T>(SqlCacheKey sqlType, string sql)
        {
            string key = GetCacheKey<T>(sqlType);
            cache[key] = sql;
        }

        private string GetCacheKey<T>(SqlCacheKey sqlType)
        {
            return $"{(short)sqlType}-{typeof(T).FullName}";
        }
    }
}
