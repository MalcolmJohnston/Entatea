using System.Data;

namespace Testadal
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
