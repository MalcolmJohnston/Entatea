using Humanizer;

namespace Entatea.Resolvers
{
    public class UnderscoreColumnNameResolver : IColumnNameResolver
    {
        public string GetColumnName(string columnName)
        {
            return columnName.Underscore();
        }
    }
}
