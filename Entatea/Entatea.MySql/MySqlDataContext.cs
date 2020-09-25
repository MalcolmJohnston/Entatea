using Entatea.Cache;
using Entatea.Resolvers;

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

        public MySqlDataContext(
            string connectionString,
            ITableNameResolver tableNameResolver,
            IColumnNameResolver columnNameResolver) : base(
            new MySqlConnectionProvider(connectionString),
            new MySqlBuilder(tableNameResolver, columnNameResolver),
            new SqlCache())
        {
        }
    }
}
