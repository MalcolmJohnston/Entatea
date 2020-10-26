using System;

namespace Entatea.Annotations
{
    /// <summary>
    /// The Discriminator attribute is used to denote a property on a POCO that is mapped to a column which indicates the type
    /// of a record in a database.
    /// For example;
    ///     If the Property 'RecordType' has the Discriminator attribute with a value of 5
    ///     Then the column mapped to RecordType would be set to 5 on insert and marked as read-only.
    ///     Any selects would automatically have added WHERE RecordType = 5.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DiscriminatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminatorAttribute"/> class.
        /// </summary>
        /// <param name="valueOnInsert">The value.</param>
        public DiscriminatorAttribute(object valueOnInsert)
        {
            this.ValueOnInsert = valueOnInsert;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object ValueOnInsert { get; private set; }
    }
}
