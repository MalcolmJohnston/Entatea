using Humanizer;

namespace Entatea.Resolvers
{
    public class DefaultTableNameResolver : ITableNameResolver
    {
        public string GetTableName(string typeName)
        {
            return typeName.Pluralize(false);
        }
    }
}
