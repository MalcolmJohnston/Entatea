namespace Testadal.Predicate
{
    public interface IFieldPredicate : IComparePredicate
    {
        string PropertyName { get; set; }
        object Value { get; set; }
    }
}
