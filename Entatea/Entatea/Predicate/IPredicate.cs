using System.Collections.Generic;

using Entatea.SqlBuilder;

namespace Entatea.Predicate
{
    public interface IPredicate
    {
        string GetSql(ISqlBuilder sqlBuilder, int parameterIndex , out int parameterCount);

        IEnumerable<KeyValuePair<string, object>> GetParameters(int parameterIndex, out int parameterCount);
    }
}
