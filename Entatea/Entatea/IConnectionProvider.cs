using System.Data;
using System.Threading.Tasks;

namespace Entatea
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();

        IDbTransaction GetTransaction();

        void CloseConnection();

        void BeginTransaction(IDataContext dataContext);

        void Commit(IDataContext dataContext);

        void Rollback(IDataContext dataContext);

        void NotifyDisposed(IDataContext dataContext);
    }
}
