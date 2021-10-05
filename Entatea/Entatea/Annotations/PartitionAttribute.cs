using System;

namespace Entatea.Annotations
{
    /// <summary>
    /// The Partition attribute is used to denote a property on a POCO that is mapped to a column 
    /// where we should return only a horizonal parition of the rows.
    /// For example;
    ///     If the Property 'Id' has the Partition with a from value of 20
    ///     Then only rows with Id 20 and above would be returned.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PartitionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionAttribute"/> class.
        /// </summary>
        /// <param name="valueOnInsert">The value.</param>
        public PartitionAttribute(object value, bool fromValue = true)
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

        public PartitionAttribute(object fromValue, object toValue)
        {
            this.FromValue = fromValue;
            this.ToValue = toValue;
        }

        public object FromValue { get; private set; }

        public object ToValue { get; private set; }
    }
}
