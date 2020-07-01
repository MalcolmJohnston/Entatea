namespace TdDb.Predicate
{
    public class Equal : SimplePredicate, IPredicate
    {
        public Equal(string columnName, string propertyName) : base(columnName, propertyName, Operator.Equal)
        {
        }
    }
}
