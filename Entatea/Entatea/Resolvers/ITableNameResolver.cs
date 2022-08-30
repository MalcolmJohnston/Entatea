using Entatea.Model;

namespace Entatea.Resolvers
{
    public interface ITableNameResolver
    {
        string GetTableName(ClassMap classMap);
    }
}
