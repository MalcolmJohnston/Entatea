using System.Data;
using Entatea;
using Entatea.Cache;

namespace Entatea.MySql
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
