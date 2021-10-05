using System;
using System.Reflection;

namespace Entatea.Model
{
    /// <summary>
    /// Class the defines a property mapping.
    /// </summary>
    public class PropertyMap
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public string PropertyName
        {
            get { return this.PropertyInfo.Name; }
        }

        public string ColumnName { get; private set; }

        public bool IsKey
        {
            get { return this.KeyType != KeyType.NotAKey; }
        }

        public KeyType KeyType { get; private set; } = KeyType.NotAKey;

        /// <summary>
        /// Gets a value indicating whether this property is editable, defaults to true.
        /// If is editable is false, then this property will be excluded from updates.
        /// </summary>
        public bool IsEditable { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether this property is read-only.
        /// This will exclude the property from inserts and updates.
        /// </summary>
        public bool IsReadOnly { get; private set; } = false;

        public bool IsRequired { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property is a date stamp and
        /// so should instance is date stamp.
        /// </summary>
        public bool IsDateStamp { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is soft delete.
        /// </summary>
        public bool IsSoftDelete { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a discriminator.
        /// </summary>
        public bool IsDiscriminator { get; private set; }

        /// <summary>
        /// Gets a value indication whether this instance is a partition.
        /// </summary>
        public bool IsPartition { get; private set; }

        /// <summary>
        /// The value the property should be set to on insert (only applies if IsSoftDelete, or IsDiscriminator)
        /// </summary>
        public object ValueOnInsert { get; private set; }

        /// <summary>
        /// The value the property should be set to on delete (only applies if IsSoftDelete)
        /// </summary>
        public object ValueOnDelete { get; private set; }

        /// <summary>
        /// The value to partition from (only applies if IsPartition, may be null)
        /// </summary>
        public object PartitionFromValue { get; private set; }

        /// <summary>
        /// The value to partition to (only applies if IsPartition, may be null)
        /// </summary>
        public object PartitionToValue { get; private set; }

        /// <summary>
        /// Loads the property map.
        /// </summary>
        /// <param name="propertyInfo">The proerty info.</param>
        /// <returns>The property map for this property.</returns>
        /// <exception cref="ArgumentException">Readonly and Editable attributes specified with opposing values.</exception>
        public static PropertyMap LoadPropertyMap(PropertyInfo propertyInfo)
        {
            // if the property info is null or not mapped attribute present
            // then return null
            if (propertyInfo == null || PropertyAttributeHelper.IsNotMapped(propertyInfo))
            {
                return null;
            }

            PropertyMap pm = new PropertyMap() { PropertyInfo = propertyInfo };

            // read the property metadata from custom attributes
            pm.KeyType = PropertyAttributeHelper.GetKeyType(propertyInfo);
            bool isKey = pm.KeyType != KeyType.NotAKey || PropertyAttributeHelper.IsKey(propertyInfo);
            
            bool isRequired = PropertyAttributeHelper.IsRequired(propertyInfo);
            bool isReadOnly = PropertyAttributeHelper.IsReadOnly(propertyInfo);
            bool isEditable = PropertyAttributeHelper.IsEditable(propertyInfo);
            bool isDateStamp = PropertyAttributeHelper.IsDateStamp(propertyInfo);
            dynamic softDeleteAttribute = PropertyAttributeHelper.GetSoftDelete(propertyInfo);
            dynamic discriminatorAttribute = PropertyAttributeHelper.GetDiscriminator(propertyInfo);
            dynamic partitionAttribute = PropertyAttributeHelper.GetPartition(propertyInfo);

            if (softDeleteAttribute != null && discriminatorAttribute != null)
            {
                throw new ArgumentException("Property cannot be soft delete and discriminator");
            }

            // use the metadata to populate the property map

            // not a key, check whether this property is an implied key
            if (!isKey)
            {
                isKey = propertyInfo.Name == "Id" || propertyInfo.Name == $"{propertyInfo.DeclaringType.Name}Id";
            }

            // key with no key type defined, then imply it
            if (isKey && pm.KeyType == KeyType.NotAKey)
            {
                if (propertyInfo.PropertyType == typeof(int))
                {
                    // if integer then treat as identity by default
                    pm.KeyType = KeyType.Identity;
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    // if guid then treat as guid
                    pm.KeyType = KeyType.Guid;
                }
                else
                {
                    // otherwise treat as assigned
                    pm.KeyType = KeyType.Assigned;
                }
            }

            // set remaining properties
            dynamic columnAttribute = PropertyAttributeHelper.GetColumnAttribute(propertyInfo);
            pm.ColumnName = columnAttribute != null ? columnAttribute.Name : string.Empty;
            pm.IsRequired = isRequired;
            pm.IsDateStamp = isDateStamp;
            pm.IsSoftDelete = softDeleteAttribute != null;
            pm.IsDiscriminator = discriminatorAttribute != null;
            pm.IsPartition = partitionAttribute != null;
            if (pm.IsSoftDelete)
            {
                pm.ValueOnInsert = softDeleteAttribute.ValueOnInsert;
                pm.ValueOnDelete = softDeleteAttribute.ValueOnDelete;
            }
            if (pm.IsDiscriminator)
            {
                pm.ValueOnInsert = discriminatorAttribute.ValueOnInsert;
            }
            if (pm.IsPartition)
            {
                pm.PartitionFromValue = partitionAttribute.FromValue;
                pm.PartitionToValue = partitionAttribute.ToValue;
            }

            // set read-only / editable
            // if property is identity, guid or sequential key then cannot be
            // editable and must be read-only
            if (pm.KeyType == KeyType.Identity || pm.KeyType == KeyType.Guid || pm.KeyType == KeyType.Sequential)
            {
                pm.IsReadOnly = true;
                pm.IsEditable = false;
            }
            else
            {
                // only take into account editable attribute if isReadOnly = false
                // i.e. cannot be editable and readonly
                pm.IsReadOnly = isReadOnly;
                pm.IsEditable = !isReadOnly && isEditable;
            }

            return pm;
        }
    }
}
