using System.Data;
using TdDb;
using TdDb.Cache;

namespace TdDb.SqlServer
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
