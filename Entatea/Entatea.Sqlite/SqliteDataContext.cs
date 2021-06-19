using Entatea.Cache;
using Entatea.Resolvers;

namespace Entatea.Sqlite
{
    public class SqliteDataContext : BaseDataContext, IDataContext
    {
        public SqliteDataContext(string connectionString) : base(
            new SqliteConnectionProvider(connectionString),
            new SqliteBuilder(),
            new SqlCache())
        {
        }

        public SqliteDataContext(SqliteConnectionProvider connectionProvider) : base(
            connectionProvider,
            new SqliteBuilder(),
            new SqlCache())
        {
        }

        public SqliteDataContext(
            string connectionString,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
            new SqliteConnectionProvider(connectionString),
            new SqliteBuilder(tableNameResolver, columnNameResolver),
            new SqlCache())
        {
        }

        public SqliteDataContext(
            SqliteConnectionProvider connectionProvider,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
                connectionProvider,
                new SqliteBuilder(tableNameResolver, columnNameResolver),
                new SqlCache())
        {
        }
    }
}
