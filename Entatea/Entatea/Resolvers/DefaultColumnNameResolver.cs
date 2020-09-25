namespace Entatea.Resolvers
{
    public class DefaultColumnNameResolver : IColumnNameResolver
    {
        public string GetColumnName(string propertyName)
        {
            return propertyName;
        }
    }
}
