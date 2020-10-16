using System.Data;
using Microsoft.Data.Sqlite;

namespace Entatea.Sqlite
{
    public class SqliteConnectionProvider : IConnectionProvider
    {
        private readonly string connectionString;
        public SqliteConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            IDbConnection conn = new SqliteConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}
