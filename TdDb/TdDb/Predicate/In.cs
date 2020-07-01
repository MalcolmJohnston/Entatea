namespace TdDb.Predicate
{
    public class In : SimplePredicate, IPredicate
    {
        public In(string columnName, string propertyName) : base(columnName, propertyName, Operator.In)
        {
        }
    }
}
