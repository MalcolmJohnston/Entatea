﻿namespace Entatea.Predicate
{
    public interface IComparePredicate : IPredicate
    {
        Operator Operator { get; set; }
        bool Not { get; set; }
    }
}
