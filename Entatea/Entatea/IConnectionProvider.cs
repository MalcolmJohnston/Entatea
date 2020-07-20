using System.Data;

namespace Entatea
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
