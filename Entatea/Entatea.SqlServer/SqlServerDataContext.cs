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

        public SqlServerDataContext(
            string connectionString,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
            new SqlServerConnectionProvider(connectionString),
            new TSqlBuilder(tableNameResolver, columnNameResolver),
            new SqlCache())
        {
        }
    }
}
