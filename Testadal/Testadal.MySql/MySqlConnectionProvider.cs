using System.Data;

using MySql.Data.MySqlClient;

namespace Testadal.MySql
{
    public class MySqlConnectionProvider : IConnectionProvider
    {
        private readonly string connectionString;
        public MySqlConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            IDbConnection conn = new MySqlConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}
