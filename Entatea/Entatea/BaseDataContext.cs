using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Entatea.Cache;
using Entatea.Model;
using Entatea.Predicate;
using Entatea.SqlBuilder;

namespace Entatea
{
    public class BaseDataContext : IDataContext
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly ISqlProvider sqlProvider;
        private readonly Guid id = Guid.NewGuid();

        public Guid Id { get { return id; } }

        public DataContextState State { get; private set; } = DataContextState.NoTransaction;

        protected IDbConnection Connection { get { return this.connectionProvider.GetConnection(this); } }

        protected IDbTransaction Transaction { get { return this.connectionProvider.GetTransaction(); } }

        public BaseDataContext(
            IConnectionProvider connectionProvider, 
            ISqlBuilder sqlBuilder, 
            ISqlCache sqlCache)
        {
            this.connectionProvider = connectionProvider;
            this.sqlProvider = new SqlProvider(sqlBuilder, sqlCache);
        }

        public async Task<T> Create<T>(T entity) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // set the value of the sequential key
            if (classMap.HasSequentialKey)
            {
                // read the next id from the database
                object id = await this.GetNextId<T>(entity);

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

            // set discriminator properties
            if(classMap.DiscriminatorProperties.Any())
            {
                foreach (PropertyMap discriminatorProperty in classMap.DiscriminatorProperties)
                {
                    discriminatorProperty.PropertyInfo.SetValue(
                        entity,
                        discriminatorProperty.ValueOnInsert);
                }
            }

            // execute the insert
            var row = (await this.Connection.QueryAsync(sqlProvider.GetInsertSql<T>(), entity, this.Transaction)
                                            .ConfigureAwait(false))
                                            .SingleOrDefault();

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

        public async Task<T> Read<T>(object id) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate that all key properties are passed
            IList<IPredicate> predicates = classMap.ValidateKeyProperties<T>(id);
            predicates = classMap.AddDefaultPredicates<T>(predicates);

            return 
                (await this.Connection.QueryAsync<T>(
                    sqlProvider.GetSelectWhereSql<T>(predicates),
                    predicates.GetParameters(), this.Transaction).ConfigureAwait(false))
                                                                 .SingleOrDefault();
        }

        public async Task<T> Read<T>(params IPredicate[] predicates) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate that all key properties are passed
            predicates = classMap.AddDefaultPredicates<T>(predicates).ToArray();

            IEnumerable<T> results = await this.Connection.QueryAsync<T>(
                this.sqlProvider.GetSelectWhereSql<T>(predicates),
                predicates.GetParameters(),
                this.Transaction).ConfigureAwait(false);

            if (results.Count() > 1)
            {
                throw new ArgumentException("Expected predicates to evaluate to a single row.");
            }

            return results.SingleOrDefault();
        }

        public async Task<IEnumerable<T>> ReadAll<T>() where T : class
        {
            // check for discriminator predicates
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            IEnumerable<IPredicate> predicates = classMap.GetDefaultPredicates<T>();

            if (predicates.Any())
            {
                return await this.Connection.QueryAsync<T>(
                    this.sqlProvider.GetSelectWhereSql<T>(predicates),
                    predicates.GetParameters(),
                    this.Transaction).ConfigureAwait(false);
            }

            return await this.Connection.QueryAsync<T>(sqlProvider.GetSelectAllSql<T>(), null, this.Transaction).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> ReadList<T>(object whereConditions) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate all properties passed
            IList<IPredicate> predicates = classMap.ValidateWhereProperties<T>(classMap.CoalesceToDictionary(whereConditions));

            return await this.ReadList<T>(predicates);
        }

        public async Task<IEnumerable<T>> ReadList<T>(params IPredicate[] predicates) where T : class
        {
            return await ReadList<T>(predicates.AsEnumerable());
        }

        public async Task<PagedList<T>> ReadList<T>(object whereConditions, object sortOrders, int pageSize, int pageNumber) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            IList<IPredicate> predicates = classMap.ValidateWhereProperties<T>(whereConditions);

            return await ReadList<T>(predicates, sortOrders, pageSize, pageNumber);
        }

        public async Task<PagedList<T>> ReadList<T>(object sortOrders, int pageSize, int pageNumber, params IPredicate[] predicates) where T : class
        {
            return await ReadList<T>(predicates, sortOrders, pageSize, pageNumber);
        }

        public async Task<T> Update<T>(object properties) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // coalesce all properties into update dictionary
            IDictionary<string, object> updDictionary = classMap.CoalesceToDictionary(properties);

            // coalesce key properties
            IDictionary<string, object> keyDictionary = classMap.CoalesceKeyToDictionary(properties);

            // remove key properties from update dictionary
            keyDictionary.Keys.ToList().ForEach(x => updDictionary.Remove(x));

            // get the parameters for the WHERE clause
            IList<IPredicate> predicates = classMap.ValidateKeyProperties<T>(properties);
            predicates = classMap.AddDefaultPredicates<T>(predicates);

            IDictionary<string, object> keyParameters = predicates.GetParameters();

            // add the WHERE clause parameters to our update dictionary
            updDictionary = updDictionary.Union(keyParameters).ToDictionary(x => x.Key, x => x.Value);

            await this.Connection.ExecuteAsync(sqlProvider.GetUpdateSql<T>(properties), updDictionary, this.Transaction)
                                 .ConfigureAwait(false);

            return await this.Read<T>(keyDictionary);
        }

        public async Task Delete<T>(object id) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate the key properties
            IList<IPredicate> predicates = classMap.ValidateKeyProperties<T>(id);
            predicates = classMap.AddDefaultPredicates<T>(predicates);

            await this.Connection.QueryAsync<T>(
                sqlProvider.GetDeleteWhereSql<T>(predicates), 
                predicates.GetParameters(), this.Transaction).ConfigureAwait(false);
        }

        public async Task DeleteList<T>(object whereConditions) where T : class
        {
            if (whereConditions == null)
            {
                throw new ArgumentException("Please pass where conditions");
            }

            // validate the properties
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            IList<IPredicate> predicates = classMap.ValidateWhereProperties<T>(whereConditions);

            await this.DeleteList<T>(predicates);
        }

        public async Task DeleteList<T>(params IPredicate[] predicates) where T : class
        {
            if (predicates == null || predicates.Length == 0)
            {
                throw new ArgumentException("Please pass where conditions");
            }

            await this.DeleteList<T>(predicates.AsEnumerable());
        }

        private async Task<IEnumerable<T>> ReadList<T>(IEnumerable<IPredicate> predicates) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();
            predicates = classMap.AddDefaultPredicates<T>(predicates);

            return await this.Connection.QueryAsync<T>(
                this.sqlProvider.GetSelectWhereSql<T>(predicates),
                predicates.GetParameters(),
                this.Transaction).ConfigureAwait(false);
        }

        private async Task<PagedList<T>> ReadList<T>(IEnumerable<IPredicate> predicates, object sortOrders, int pageSize, int pageNumber) where T : class
        {
            // create the paging variables
            int firstRow = ((pageNumber - 1) * pageSize) + 1;
            int lastRow = firstRow + (pageSize - 1);

            ClassMap classMap = ClassMapper.GetClassMap<T>();
            predicates = classMap.AddDefaultPredicates<T>(predicates);

            // get the parameters
            var parameters = predicates.GetParameters();

            // read the count
            int total = await this.Connection.ExecuteScalarAsync<int>(sqlProvider.GetSelectCountSql<T>(predicates), parameters, this.Transaction);

            // read the rows
            IEnumerable<T> results = await this.Connection.QueryAsync<T>(
                sqlProvider.GetSelectWhereSql<T>(predicates, sortOrders, firstRow, lastRow),
                parameters,
                this.Transaction);

            return new PagedList<T>()
            {
                Rows = results,
                HasNext = lastRow < total,
                HasPrevious = firstRow > 1,
                TotalPages = (total / pageSize) + ((total % pageSize) > 0 ? 1 : 0),
                TotalRows = total
            };
        }

        private async Task DeleteList<T>(IEnumerable<IPredicate> predicates) where T : class
        {
            if (predicates.Count() == 0)
            {
                throw new ArgumentException("Please pass where conditions.");
            }

            ClassMap classMap = ClassMapper.GetClassMap<T>();
            predicates = classMap.AddDefaultPredicates<T>(predicates);

            await this.Connection.QueryAsync<T>(sqlProvider.GetDeleteWhereSql<T>(predicates), predicates.GetParameters(), this.Transaction)
                                 .ConfigureAwait(false);
        }

        private async Task<object> GetNextId<T>(T entity) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // get the predicates
            List<IPredicate> predicates = classMap.ValidateAssignedKeyProperties<T>(entity).ToList();
            predicates.AddRange(classMap.GetSequentialPartitionPredicates<T>());

            // get the parameters
            var parameters = predicates.GetParameters();

            object id = await this.Connection.ExecuteScalarAsync(
                sqlProvider.GetSelectNextIdSql<T>(),
                parameters,
                this.Transaction).ConfigureAwait(false);

            if (classMap.HasSequentialPartitionKey)
            {
                id = classMap.CoalesceNextSequentialPartitionId(id);
            }

            return id;
        }

        public void BeginTransaction()
        {
            this.connectionProvider.BeginTransaction(this);
            this.State = DataContextState.InTransaction;
        }

        public void Commit()
        {
            if (this.connectionProvider.Commit(this))
            {
                this.State = DataContextState.Committed;
            }
        }

        public void Rollback()
        {
            try
            {
                this.connectionProvider.Rollback(this);
            }
            finally
            {
                this.State = DataContextState.Rolledback;
            }
        }

        public void Dispose()
        {
           this.connectionProvider.NotifyDisposed(this);
        }
    }
}
