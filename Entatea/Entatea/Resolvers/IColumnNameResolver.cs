using Entatea.Model;

namespace Entatea.Resolvers
{
    public interface IColumnNameResolver
    {
        string GetColumnName(ClassMap classMap, string propertyName);
    }
}
