using System.Data;
using Entatea;
using Entatea.Cache;

namespace Entatea.SqlServer
{
    public class SqlServerDataContext : BaseDataContext, IDataContext
    {
        public SqlServerDataContext(string connectionString) : base(
            new SqlServerConnectionProvider(connectionString),
            new TSqlBuilder(),
            new SqlCache())
        {
        }
    }
}
