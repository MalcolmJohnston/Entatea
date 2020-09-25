using Humanizer;

namespace Entatea.Resolvers
{
    public class UnderscoreTableNameResolver : ITableNameResolver
    {
        public string GetTableName(string typeName)
        {
            return typeName.Underscore().Pluralize();
        }
    }
}
