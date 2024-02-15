using System;

namespace Entatea.Annotations
{
    /// <summary>
    /// Optional Editable attribute.
    /// Entatea treats all properties as editable by default use the editable attribute with false to specify they are not editable.
    /// You can use the System.ComponentModel.DataAnnotations version in its place.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute
    {
        /// <summary>
        /// Optional Editable attribute.
        /// </summary>
        /// <param name="iseditable"></param>
        public EditableAttribute(bool iseditable)
        {
            AllowEdit = iseditable;
        }
        /// <summary>
        /// Does this property persist to the database?
        /// </summary>
        public bool AllowEdit { get; private set; }
    }
}
