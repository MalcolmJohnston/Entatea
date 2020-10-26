using System.Collections.Generic;

using Entatea.SqlBuilder;

namespace Entatea.Predicate
{
    public class FieldPredicate<T> : ComparePredicate, IFieldPredicate
        where T : class
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public string GetSql(ISqlBuilder sqlBuilder, int parameterIndex, out int parameterCount)
        {
            parameterCount = 1;

            string columnName = sqlBuilder.GetColumnName<T>(PropertyName);
            if (Value == null)
            {
                return $"{columnName} IS {(Not ? "NOT " : string.Empty)}NULL";
            }

            if (this.Operator == Operator.Contains)
            {
                return $"{columnName} {GetOperatorString()} {sqlBuilder.CallConcatenate("'%'", $"@p{parameterIndex}", "'%'")}";
            }
            else if (this.Operator == Operator.StartsWith)
            {
                return $"{columnName} {GetOperatorString()} {sqlBuilder.CallConcatenate($"@p{parameterIndex}", "'%'")}";
            }
            else if (this.Operator == Operator.EndsWith)
            {
                return $"{columnName} {GetOperatorString()} {sqlBuilder.CallConcatenate("'%'", $"@p{parameterIndex}")}";
            }
            return $"{columnName} {GetOperatorString()} @p{parameterIndex}";
        }

        public IEnumerable<KeyValuePair<string, object>> GetParameters(int parameterIndex, out int parameterCount)
        {
            parameterCount = 1;

            return new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>($"@p{parameterIndex}", this.Value)
            };
        }
    }
}
