using System;

namespace Entatea.Annotations
{
    /// <summary>
    /// Used to mark a property as a date stamp.
    /// TdDb will automatically set the date of the property on insert, update or soft delete.
    /// TdDb will not update the property on update or delete if the column is not editable (i.e. an insert date)
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateStampAttribute : Attribute
    {
    }
}
