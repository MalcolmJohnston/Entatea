namespace TdDb.Predicate
{
    public interface IPredicate
    {
        string ColumnName { get; }

        string PropertyName { get; }

        Operator Operator { get; }
    }
}
