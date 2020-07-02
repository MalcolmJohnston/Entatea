using System.Data;
using Testadal;
using Testadal.Cache;

namespace Testadal.MySql
{
    public class MySqlDataContext : BaseDataContext, IDataContext
    {
        public MySqlDataContext(string connectionString) : base(
            new MySqlConnectionProvider(connectionString),
            new MySqlBuilder(),
            new SqlCache())
        {
        }
    }
}
