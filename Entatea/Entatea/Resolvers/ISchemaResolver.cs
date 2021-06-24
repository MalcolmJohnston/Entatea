using Entatea.Model;

namespace Entatea.Resolvers
{
    public interface ISchemaResolver
    {
        string GetSchema(ClassMap classMap);
    }
}
