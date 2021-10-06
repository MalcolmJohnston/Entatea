using System;

namespace Entatea.Annotations
{
    /// <summary>
    /// The Sequential Partition Key attribute is used to designate a sequential key that has
    /// a lower and upper bound.
    /// Bounds can be defined as;
    /// + Lower without upper e.g. lower to Int.Max
    /// + Upper without lower e.g. 1 to upper
    /// + Lower and upper (and intersection)
    /// For example;
    ///     If the Property 'Id' has the Sequential Parition Key with a from value of 20
    ///     Then only rows with Id 20 and above would be returned.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SequentialPartitionKeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialPartitionKeyAttribute"/> class.
        /// </summary>
        /// <param name="valueOnInsert">The value.</param>
        public SequentialPartitionKeyAttribute(long value, bool fromValue = true)
        {
            if (fromValue)
            {
                this.FromValue = value;
            } 
            else
            {
                this.ToValue = value;
            }
        }

        public SequentialPartitionKeyAttribute(long fromValue, long toValue)
        {
            this.FromValue = fromValue;
            this.ToValue = toValue;
        }

        public long? FromValue { get; private set; }

        public long? ToValue { get; private set; }
    }
}
