using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Entatea.Predicate;

namespace Entatea
{
    public interface IDataContext : IDisposable
    {
        DataContextState State { get; }

        void BeginTransaction();

        void Commit();

        void Rollback();

        Task<T> Create<T>(T entity) where T : class;

        Task<T> Read<T>(object id) where T : class;

        Task<T> Read<T>(params IPredicate[] predicates) where T : class;

        Task<IEnumerable<T>> ReadAll<T>() where T : class;

        Task<IEnumerable<T>> ReadList<T>(object whereConditions) where T : class;

        Task<IEnumerable<T>> ReadList<T>(params IPredicate[] predicates) where T : class;

        Task<PagedList<T>> ReadList<T>(object whereConditions, object sortOrders, int pageSize, int pageNumber) where T : class;

        Task<PagedList<T>> ReadList<T>(object sortOrders, int pageSize, int pageNumber, params IPredicate[] predicates) where T : class;

        Task<T> Update<T>(object properties) where T : class;

        Task Delete<T>(object id) where T : class;

        Task DeleteList<T>(object whereConditions) where T : class;

        Task DeleteList<T>(params IPredicate[] predicates) where T : class;

        Task HardDelete<T>(object id) where T : class;

        Task HardDeleteList<T>(object whereConditions) where T : class;

        Task HardDeleteList<T>(params IPredicate[] predicates) where T : class;
    }
}
