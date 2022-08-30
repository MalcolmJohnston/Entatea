using Humanizer;

using Entatea.Model;

namespace Entatea.Resolvers
{
    public class DefaultTableNameResolver : ITableNameResolver
    {
        public string GetTableName(ClassMap classMap)
        {
            return classMap.Name.Pluralize(false);
        }
    }
}
