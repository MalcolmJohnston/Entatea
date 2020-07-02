using System.Data;
using Testadal;
using Testadal.Cache;

namespace Testadal.SqlServer
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
