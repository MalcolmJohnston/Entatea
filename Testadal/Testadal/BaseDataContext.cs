using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Testadal.Cache;
using Testadal.Model;
using Testadal.SqlBuilder;

namespace Testadal
{
    public class BaseDataContext : IDataContext
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly ISqlProvider sqlProvider;

        public BaseDataContext(
            IConnectionProvider connectionProvider, 
            ISqlBuilder sqlBuilder, 
            ISqlCache sqlCache)
        {
            this.connectionProvider = connectionProvider;
            this.sqlProvider = new SqlProvider(sqlBuilder, sqlCache);
        }

        public async Task<T> Create<T>(T entity)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            using (IDbConnection conn = connectionProvider.GetConnection())
            {
                // set the value of the sequential key
                if (classMap.HasSequentialKey)
                {
                    // read the next id from the database
                    object id = await conn.ExecuteScalarAsync(sqlProvider.GetSelectNextIdSql<T>(), entity).ConfigureAwait(false);

                    // set the sequential key on our entity
                    classMap.SequentialKey.PropertyInfo.SetValue(
                        entity,
                        Convert.ChangeType(id, classMap.SequentialKey.PropertyInfo.PropertyType));
                }

                // set current date time on any date stamp properties
                if (classMap.DateStampProperties.Any())
                {
                    DateTime dateStamp = DateTime.Now;
                    foreach (PropertyMap dateStampProperty in classMap.DateStampProperties)
                    {
                        dateStampProperty.PropertyInfo.SetValue(entity, dateStamp);
                    }
                }

                // set value on insert on any soft delete properties
                if (classMap.IsSoftDelete)
                {
                    classMap.SoftDeleteProperty.PropertyInfo.SetValue(
                        entity,
                        classMap.SoftDeleteProperty.ValueOnInsert);
                }

                // execute the insert
                var row = (await conn.QueryAsync(sqlProvider.GetInsertSql<T>(), entity).ConfigureAwait(false)).SingleOrDefault();

                // apply OUTPUT values to identity column
                if (classMap.HasIdentityKey)
                {
                    if (row == null)
                    {
                        throw new DataException("Expected row with Identity values, but no row returned.");
                    }

                    // convert to dictionary to iterate through results
                    IDictionary<string, object> rowDictionary = (IDictionary<string, object>)row;

                    // in MySQL identity values from LAST_INSERT_ID() come back as ulong
                    // but this may not be the type of our identity property so cast
                    object identityValue = Convert.ChangeType(
                        rowDictionary[classMap.IdentityKey.PropertyName],
                        classMap.IdentityKey.PropertyInfo.PropertyType);

                    // set the key value on the entity
                    classMap.IdentityKey.PropertyInfo.SetValue(entity, identityValue);
                }

                return entity;
            }
        }

        public async Task<T> Read<T>(object id)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate that all key properties are passed
            id = classMap.ValidateKeyProperties(id);

            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                return (await conn.QueryAsync<T>(sqlProvider.GetSelectByIdSql<T>(), id)
                    .ConfigureAwait(false))
                    .SingleOrDefault();
            }
        }

        public async Task<IEnumerable<T>> ReadAll<T>()
        {
            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                return await conn.QueryAsync<T>(sqlProvider.GetSelectAllSql<T>())
                    .ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> ReadList<T>(object whereConditions)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate all properties passed
            classMap.ValidateWhereProperties(classMap.CoalesceToDictionary(whereConditions));

            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                return await conn.QueryAsync<T>(
                    this.sqlProvider.GetSelectWhereSql<T>(whereConditions),
                    whereConditions).ConfigureAwait(false);
            }
        }

        public async Task<PagedList<T>> ReadList<T>(object whereConditions, object sortOrders, int pageSize, int pageNumber)
        {
            // create the paging variables
            int firstRow = ((pageNumber - 1) * pageSize) + 1;
            int lastRow = firstRow + (pageSize - 1);

            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                // read the count
                int total = await conn.ExecuteScalarAsync<int>(sqlProvider.GetSelectCountSql<T>(whereConditions), whereConditions);

                // read the rows
                IEnumerable<T> results = await conn.QueryAsync<T>(sqlProvider.GetSelectWhereSql<T>(whereConditions, sortOrders, firstRow, lastRow), whereConditions);

                return new PagedList<T>()
                {
                    Rows = results,
                    HasNext = lastRow < total,
                    HasPrevious = firstRow > 1,
                    TotalPages = (total / pageSize) + ((total % pageSize) > 0 ? 1 : 0),
                    TotalRows = total
                };
            }
        }

        public async Task<T> Update<T>(object properties)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // coalesce key property
            IDictionary<string, object> id = classMap.CoalesceKeyToDictionary(properties);

            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                // execute the update
                await conn.ExecuteAsync(sqlProvider.GetUpdateSql<T>(properties), properties).ConfigureAwait(false);

                return (await conn.QueryAsync<T>(sqlProvider.GetSelectByIdSql<T>(), id).ConfigureAwait(false)).SingleOrDefault();
            }
        }

        public async Task Delete<T>(object id)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate the key properties
            id = classMap.ValidateKeyProperties(id);

            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                await conn.QueryAsync<T>(sqlProvider.GetDeleteByIdSql<T>(), id).ConfigureAwait(false);
            }
        }

        public async Task DeleteList<T>(object whereConditions)
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate the key properties
            classMap.ValidateWhereProperties(whereConditions);

            using (IDbConnection conn = this.connectionProvider.GetConnection())
            {
                // delete
                await conn.QueryAsync<T>(sqlProvider.GetDeleteWhereSql<T>(whereConditions), whereConditions).ConfigureAwait(false);
            }
        }
    }
}
