using Entatea.Cache;
using Entatea.Resolvers;

namespace Entatea.SqlServer
{
    public class SqlServerDataContext : BaseDataContext, IDataContext
    {
        public SqlServerDataContext(string connectionString) : base(
            new SqlServerConnectionProvider(connectionString),
            new TSqlBuilder(),
            new SqlCache())
        {
        }

        public SqlServerDataContext(SqlServerConnectionProvider connectionProvider) : base(
            connectionProvider,
            new TSqlBuilder(),
            new SqlCache())
        {
        }

        public SqlServerDataContext(
            string connectionString,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
            new SqlServerConnectionProvider(connectionString),
            new TSqlBuilder(tableNameResolver, columnNameResolver),
            new SqlCache())
        {
        }

        public SqlServerDataContext(
            string connectionString,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver,
            string defaultSchema) : base(
            new SqlServerConnectionProvider(connectionString),
            new TSqlBuilder(tableNameResolver, columnNameResolver, defaultSchema),
            new SqlCache())
        {
        }

        public SqlServerDataContext(
            SqlServerConnectionProvider connectionProvider,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver,
            string defaultSchema) : base(
                connectionProvider,
                new TSqlBuilder(tableNameResolver, columnNameResolver, defaultSchema),
                new SqlCache())
        {
        }

        public SqlServerDataContext(
            string connectionString,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver,
            ISchemaResolver schemaResolver) : base(
            new SqlServerConnectionProvider(connectionString),
            new TSqlBuilder(tableNameResolver, columnNameResolver, schemaResolver),
            new SqlCache())
        {
        }

        public SqlServerDataContext(
            SqlServerConnectionProvider connectionProvider,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver,
            ISchemaResolver schemaResolver) : base(
                connectionProvider,
                new TSqlBuilder(tableNameResolver, columnNameResolver, schemaResolver),
                new SqlCache())
        {
        }
    }
}
