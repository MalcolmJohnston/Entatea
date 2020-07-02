using System.Collections.Generic;
using System.Threading.Tasks;

namespace Testadal
{
    public interface IDataContext
    {
        Task<T> Create<T>(T entity);

        Task<T> Read<T>(object id);

        Task<IEnumerable<T>> ReadAll<T>();

        Task<IEnumerable<T>> ReadList<T>(object whereConditions);

        Task<PagedList<T>> ReadList<T>(object whereConditions, object sortOrders, int pageSize, int pageNumber);

        Task<T> Update<T>(object properties);

        Task Delete<T>(object id);

        Task DeleteList<T>(object whereConditions);
    }
}
