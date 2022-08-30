using Entatea.Model;

namespace Entatea.Resolvers
{
    public class DefaultColumnNameResolver : IColumnNameResolver
    {
        public string GetColumnName(ClassMap classMap, string propertyName)
        {
            return propertyName;
        }
    }
}
