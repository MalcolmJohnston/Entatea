using System.Data;
using Microsoft.Data.Sqlite;

namespace Entatea.Sqlite
{
    public class SqliteConnectionProvider : BaseConnectionProvider, IConnectionProvider
    {
        public SqliteConnectionProvider(string connectionString) : base(connectionString)
        {
        }

        protected override IDbConnection GetOpenConnection()
        {
            IDbConnection conn = new SqliteConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}
