namespace Testadal.Predicate
{
    public abstract class ComparePredicate
    {
        public Operator Operator { get; set; }
        public bool Not { get; set; }

        public virtual string GetOperatorString()
        {
            switch (Operator)
            {
                case Operator.GreaterThan:
                    return Not ? "<=" : ">";
                case Operator.GreaterThanOrEqual:
                    return Not ? "<" : ">=";
                case Operator.LessThan:
                    return Not ? ">=" : "<";
                case Operator.LessThanOrEqual:
                    return Not ? ">" : "<=";
                case Operator.Contains:
                case Operator.StartsWith:
                case Operator.EndsWith:
                    return Not ? "NOT LIKE" : "LIKE";
                case Operator.In:
                    return Not ? "NOT IN" : "IN";
                default:
                    return Not ? "<>" : "=";
            }
        }
    }
}
