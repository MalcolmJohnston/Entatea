namespace Testadal.Predicate
{
    public interface IFieldPredicate : IComparePredicate
    {
        object Value { get; set; }
    }
}
