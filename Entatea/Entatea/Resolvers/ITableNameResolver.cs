using System;

namespace Entatea.Resolvers
{
    public interface ITableNameResolver
    {
        string GetTableName(string typeName);
    }
}
