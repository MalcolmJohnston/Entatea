using System.Data;
using Microsoft.Data.SqlClient;

namespace Entatea.SqlServer
{
    public class SqlServerConnectionProvider : BaseConnectionProvider, IConnectionProvider
    {
        public SqlServerConnectionProvider(string connectionString) : base(connectionString)
        {
        }

        protected override IDbConnection GetOpenConnection()
        {
            SqlConnection conn = new SqlConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}
