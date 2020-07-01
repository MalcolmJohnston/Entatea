namespace TdDb.Predicate
{
    public class SimplePredicate : IPredicate
    {
        public SimplePredicate(string columnName, string propertyName, Operator @operator)
        {
            this.ColumnName = columnName;
            this.PropertyName = propertyName;
            this.Operator = @operator;
        }
        
        public string ColumnName { get; }
        public string PropertyName { get; }
        public Operator Operator { get; }
    }
}
