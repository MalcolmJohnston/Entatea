using Humanizer;

using Entatea.Model;

namespace Entatea.Resolvers
{
    public class UnderscoreTableNameResolver : ITableNameResolver
    {
        public string GetTableName(ClassMap classMap)
        {
            return classMap.Name.Underscore().Pluralize();
        }
    }
}
