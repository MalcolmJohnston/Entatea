using Entatea.Model;

namespace Entatea.Resolvers
{
    public class DefaultSchemaResolver : ISchemaResolver
    {
        private readonly string defaultSchema;

        public DefaultSchemaResolver(string defaultSchema)
        {
            this.defaultSchema = defaultSchema;
        }

        public string GetSchema(ClassMap classMap)
        {
            if (!string.IsNullOrWhiteSpace(classMap.ExplicitSchema))
            {
                return classMap.ExplicitSchema;
            }

            return this.defaultSchema;
        }
    }
}
