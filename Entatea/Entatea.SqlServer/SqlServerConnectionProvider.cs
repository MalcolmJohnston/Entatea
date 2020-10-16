﻿using System.Data;
using System.Data.SqlClient;

namespace Entatea.SqlServer
{
    public class SqlServerConnectionProvider : IConnectionProvider
    {
        private readonly string connectionString;
        public SqlServerConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            IDbConnection conn = new SqlConnection(this.connectionString);
            conn.Open();

            return conn;
        }
    }
}
