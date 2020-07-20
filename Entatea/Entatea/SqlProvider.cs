using System;
using System.Collections.Generic;

using Entatea.Cache;
using Entatea.Predicate;
using Entatea.SqlBuilder;

namespace Entatea
{
    /// <summary>
    /// The Sql Provider class is a wrapper around the ISqlBuilder and ISqlCache implementations.
    /// It caches static SQL expressions and skips the cache for dynamic SQL expressions.
    /// </summary>
    public class SqlProvider : ISqlProvider
    {
        private readonly ISqlBuilder builder;
        private readonly ISqlCache cache;

        public SqlProvider(ISqlBuilder builder, ISqlCache cache)
        {
            this.builder = builder;
            this.cache = cache;
        }

        public string GetDeleteByIdSql<T>() where T : class
        {
            return GetFromOrAddToCache(cache.GetDeleteByIdSql<T>, builder.GetDeleteByIdSql<T>, cache.SetDeleteByIdSql<T>);
        }

        public string GetDeleteWhereSql<T>(IEnumerable<IPredicate> whereConditions) where T : class
        {
            return builder.GetDeleteWhereSql<T>(whereConditions);
        }

        public string GetInsertSql<T>() where T : class
        {
            return GetFromOrAddToCache(cache.GetInsertSql<T>, builder.GetInsertSql<T>, cache.SetInsertSql<T>);
        }

        public string GetSelectNextIdSql<T>() where T : class
        {
            return GetFromOrAddToCache(cache.GetSelectNextIdSql<T>, builder.GetSelectNextIdSql<T>, cache.SetSelectNextIdSql<T>);
        }

        public string GetSelectAllSql<T>() where T : class
        {
            return GetFromOrAddToCache(cache.GetSelectAllSql<T>, builder.GetSelectAllSql<T>, cache.SetSelectAllSql<T>);
        }

        public string GetSelectByIdSql<T>() where T : class
        {
            return GetFromOrAddToCache(cache.GetSelectByIdSql<T>, builder.GetSelectByIdSql<T>, cache.SetSelectByIdSql<T>);
        }

        public string GetSelectCountSql<T>(IEnumerable<IPredicate> whereConditions) where T : class
        {
            return builder.GetSelectCountSql<T>(whereConditions);
        }

        public string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions) where T : class
        {
            return builder.GetSelectWhereSql<T>(whereConditions);
        }

        public string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions, object sortOrders, int firstRow, int lastRow) where T : class
        {
            return builder.GetSelectWhereSql<T>(whereConditions, sortOrders, firstRow, lastRow);
        }

        public string GetUpdateSql<T>(object updateProperties) where T : class
        {
            return builder.GetUpdateSql<T>(updateProperties);
        }

        private string GetFromOrAddToCache(Func<string> getFromCache, Func<string> getFromBuilder, Action<string> addToCache)
        {
            string sql = getFromCache();
            if (string.IsNullOrWhiteSpace(sql))
            {
                sql = getFromBuilder();
                addToCache(sql);
            }

            return sql;
        }
    }
}
