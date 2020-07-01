using System.Data;

namespace TdDb
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
