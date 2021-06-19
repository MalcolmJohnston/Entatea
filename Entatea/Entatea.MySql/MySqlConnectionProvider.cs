using System.Data;

using MySql.Data.MySqlClient;

namespace Entatea.MySql
{
    public class MySqlConnectionProvider : BaseConnectionProvider, IConnectionProvider
    {
        public MySqlConnectionProvider(string connectionString) : base(connectionString)
        {
        }

        protected override IDbConnection GetOpenConnection()
        {
            IDbConnection conn = new MySqlConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}
