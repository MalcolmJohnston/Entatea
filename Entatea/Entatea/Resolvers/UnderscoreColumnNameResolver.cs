using Humanizer;

using Entatea.Model;

namespace Entatea.Resolvers
{
    public class UnderscoreColumnNameResolver : IColumnNameResolver
    {
        public string GetColumnName(ClassMap classMap, string propertyName)
        {
            return propertyName.Underscore();
        }
    }
}
