using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

using System.Reflection;
using System.Threading.Tasks;

using Testadal.Model;

namespace Testadal.InMemory
{
    /// <summary>
    /// In memory data context.  Provided to support a stub based approach to unit testing.
    /// Can also be used for quick proof of concepts.
    /// </summary>
    /// <seealso cref="Dapper.SuaveExtensions.DataContext.IDataContext" />
    public class InMemoryDataContext : IDataContext
    {
        private readonly ConcurrentDictionary<string, IList> dataStore = new ConcurrentDictionary<string, IList>();

        /// <summary>
        /// Adds test data for the specific classMap.
        /// If data already exists it will be overwritten.
        /// </summary>
        /// <typeparam name="T">The type of the test data.</typeparam>
        /// <param name="data">The data.</param>
        public void AddOrUpdate<T>(IEnumerable<T> data) where T : class
        {
            this.dataStore[typeof(T).FullName] = new List<T>(data);
        }

        /// <inheritdoc />
        public Task<T> Create<T>(T entity) where T : class
        {
            // get the type map
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // get the current list for this type
            IList<T> list = this.GetData<T>();
            IQueryable<T> linqList = list.AsQueryable();

            // if we have a sequential key then set the value
            if (classMap.HasSequentialKey)
            {
                // alias the property info
                PropertyInfo pi = classMap.SequentialKey.PropertyInfo;

                // initialise the key value
                dynamic keyValue = Activator.CreateInstance(pi.PropertyType);

                // check for existing entities
                if (list.Any())
                {
                    List<T> existingEntities = new List<T>(list);
                    if (classMap.AssignedKeys.Count() > 0)
                    {
                        // if this type has assigned keys then filter out objects from our candidates that do not have
                        // matching assigned keys
                        IDictionary<string, object> assignedValues = classMap.AssignedKeys
                            .Select(kvp => new KeyValuePair<string, object>(kvp.PropertyName, kvp.PropertyInfo.GetValue(entity)))
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                        existingEntities = this.ReadList<T>(assignedValues).GetAwaiter().GetResult().ToList();
                    }

                    // now get the last item that was added to the list in order of the key
                    T lastIn = existingEntities.AsQueryable().OrderBy($"{pi.Name} DESC").FirstOrDefault();
                    keyValue = lastIn != null ? pi.GetValue(lastIn) : keyValue;
                }

                // increment and set the key value on the object
                Expression incrementExpr = Expression.Increment(Expression.Constant(keyValue));
                pi.SetValue(entity, Expression.Lambda(incrementExpr).Compile().DynamicInvoke());
            }
            else if (classMap.HasIdentityKey)
            {
                // alias the property info
                PropertyInfo pi = classMap.IdentityKey.PropertyInfo;

                // initialise the key value to zero for the key type
                dynamic keyValue = Activator.CreateInstance(pi.PropertyType);

                // now get the last item that was added to the list in order of the key
                T lastIn = linqList.OrderBy($"{pi.Name} DESC").FirstOrDefault();
                keyValue = lastIn != null ? pi.GetValue(lastIn) : keyValue;

                // increment and set the key value on the object
                Expression incrementExpr = Expression.Increment(Expression.Constant(keyValue));
                pi.SetValue(entity, Expression.Lambda(incrementExpr).Compile().DynamicInvoke());
            }

            // now set any date stamp properties
            if (classMap.DateStampProperties.Any())
            {
                DateTime timeStamp = DateTime.Now;
                foreach (PropertyMap dateStampProperty in classMap.DateStampProperties)
                {
                    dateStampProperty.PropertyInfo.SetValue(entity, timeStamp);
                }
            }

            // and any soft delete properties
            if (classMap.IsSoftDelete)
            {
                classMap.SoftDeleteProperty.PropertyInfo.SetValue(
                    entity,
                    classMap.SoftDeleteProperty.ValueOnInsert);
            }

            // finally add the item to the list
            list.Add(entity);

            return Task.FromResult(entity);
        }

        /// <inheritdoc />
        public Task Delete<T>(object id) where T : class
        {
            // get the existing object
            T obj = this.Read<T>(id).GetAwaiter().GetResult();

            // check the object is not null (i.e. it exists in the collection)
            if (obj != null)
            {
                // remove the object from the collection
                IList<T> list = this.GetData<T>();
                list.Remove(obj);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteList<T>(object whereConditions) where T : class
        {
            // check we have some conditions
            IDictionary<string, object> whereDict = ClassMapper.GetClassMap<T>().CoalesceToDictionary(whereConditions);
            if (whereDict.Count == 0)
            {
                throw new ArgumentException("Please pass where conditions.");
            }

            IEnumerable<T> objects = new List<T>(this.ReadList<T>(whereDict).GetAwaiter().GetResult());
            if (objects.Count() > 0)
            {
                IList<T> list = this.GetData<T>();
                foreach (T obj in objects)
                {
                    list.Remove(obj);
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<T> Read<T>(object id) where T : class
        {
            // get the type map
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate the key
            IDictionary<string, object> key = classMap.CoalesceKeyToDictionary(id);

            return Task.FromResult(this.ReadWhere<T>(key).SingleOrDefault());
        }

        /// <inheritdoc />
        public Task<IEnumerable<T>> ReadAll<T>() where T : class
        {
            if (this.dataStore.ContainsKey(typeof(T).FullName))
            {
                return Task.FromResult((IEnumerable<T>)this.dataStore[typeof(T).FullName]);
            }

            return Task.FromResult(new T[0].AsEnumerable());
        }

        /// <inheritdoc />
        public Task<IEnumerable<T>> ReadList<T>(object whereConditions) where T : class
        {
            // get the type map
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate the where conditions
            whereConditions = classMap.CoalesceToDictionary(whereConditions);

            return Task.FromResult(this.ReadWhere<T>((IDictionary<string, object>)whereConditions));
        }

        /// <inheritdoc />
        public Task<PagedList<T>> ReadList<T>(object whereConditions, object sortOrders, int pageSize, int pageNumber) where T : class
        {
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // create the paging variables
            int firstRow = ((pageNumber - 1) * pageSize) + 1;
            int lastRow = firstRow + (pageSize - 1);

            // get the candidate objects
            IEnumerable<T> filteredT;
            IDictionary<string, object> whereDict = classMap.CoalesceToDictionary(whereConditions);

            if (whereDict.Count == 0)
            {
                filteredT = this.ReadAll<T>().GetAwaiter().GetResult();
            }
            else
            {
                filteredT = this.ReadList<T>(whereConditions).GetAwaiter().GetResult();
            }

            // get the total number of candidate objects
            int total = filteredT.Count();

            // validate / build the ordering string
            string ordering = string.Empty;
            IDictionary<string, SortOrder> sortOrderDict = classMap.CoalesceSortOrderDictionary(sortOrders);

            for (int i = 0; i < sortOrderDict.Count; i++)
            {
                // check whether this property exists for the type
                string propertyName = sortOrderDict.Keys.ElementAt(i);
                if (!classMap.AllProperties.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Failed to find property {propertyName} on {classMap.Name}");
                }

                ordering += string.Format(
                    "{0}{1}{2}",
                    propertyName,
                    sortOrderDict[propertyName] == SortOrder.Descending ? " desc" : string.Empty,
                    i != sortOrderDict.Count - 1 ? "," : string.Empty);
            }

            // order the rows and take the results for this page
            filteredT = filteredT.AsQueryable<T>().OrderBy(ordering).Skip(firstRow - 1).Take(pageSize);

            return Task.FromResult(new PagedList<T>()
            {
                Rows = filteredT,
                HasNext = lastRow < total,
                HasPrevious = firstRow > 1,
                TotalPages = (total / pageSize) + ((total % pageSize) > 0 ? 1 : 0),
                TotalRows = total
            });
        }

        /// <inheritdoc />
        public Task<T> Update<T>(object properties) where T : class
        {
            // get the type map
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // validate the key
            IDictionary<string, object> id = classMap.CoalesceKeyToDictionary(properties);

            // get the existing object
            T obj = this.ReadWhere<T>(id).SingleOrDefault();

            if (obj != null)
            {
                // find the properties to update
                IDictionary<string, object> allProps = classMap.CoalesceToDictionary(properties);
                IDictionary<string, object> updateProps = allProps.Where(kvp => !id.ContainsKey(kvp.Key))
                                                                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // check whether there are any properties to update
                if (!classMap.UpdateableProperties.Any(x => updateProps.Keys.Contains(x.PropertyName)))
                {
                    throw new ArgumentException("Please provide one or more updateable properties.");
                }

                // update the properties that are not date stamps
                foreach (string propertyName in updateProps.Keys)
                {
                    PropertyMap propertyMap = classMap.UpdateableProperties.Where(x => x.PropertyName == propertyName).SingleOrDefault();
                    if (propertyMap != null)
                    {
                        propertyMap.PropertyInfo.SetValue(obj, updateProps[propertyName]);
                    }
                }

                // update the properties that are date stamps (and updateable)
                if (classMap.DateStampProperties.Any(x => !x.IsReadOnly))
                {
                    DateTime dateStamp = DateTime.Now;
                    foreach (PropertyMap propertyMap in classMap.DateStampProperties.Where(x => !x.IsReadOnly))
                    {
                        propertyMap.PropertyInfo.SetValue(obj, dateStamp);
                    }
                }
            }

            return Task.FromResult(obj);
        }

        private IList<T> GetData<T>() where T : class
        {
            string cacheKey = typeof(T).FullName;

            if (!this.dataStore.ContainsKey(cacheKey))
            {
                this.dataStore[cacheKey] = new List<T>();
            }

            return this.dataStore[cacheKey] as IList<T>;
        }

        private IEnumerable<T> ReadWhere<T>(IDictionary<string, object> properties) where T : class
        {
            // get the type map
            ClassMap classMap = ClassMapper.GetClassMap<T>();

            // get the data to query
            IQueryable<T> data = this.GetData<T>().AsQueryable<T>();

            // return an empty enumerable if no objects in collection
            if (data.Count() == 0)
            {
                return new T[0];
            }

            // x =>
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            BinaryExpression body = null;
            foreach (string propertyName in properties.Keys)
            {
                // add a paramater equals expression for each property in the property bag
                PropertyMap pm = classMap.AllProperties[propertyName];

                // x.Property
                MemberExpression member = Expression.Property(parameter, pm.PropertyName);
                ConstantExpression constant = Expression.Constant(properties[propertyName]);

                // x.Property = Value
                if (body == null)
                {
                    body = Expression.Equal(member, constant);
                }
                else
                {
                    body = Expression.AndAlso(body, Expression.Equal(member, constant));
                }
            }

            var finalExpression = Expression.Lambda<Func<T, bool>>(body, parameter);

            return data.Where(finalExpression).AsEnumerable<T>();
        }
    }
}
