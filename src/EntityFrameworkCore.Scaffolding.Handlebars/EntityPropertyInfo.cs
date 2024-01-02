namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Entity property info.
    /// </summary>
    public class EntityPropertyInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityPropertyInfo() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyIsNullable">Property is nullable.</param>
        /// <param name="isEnumPropertyType">Is Enumeration Property Type</param>
        /// <param name="propertyDefaultEnumValue">Default Enumeration Value. Format will be EnumName.EnumValue</param>
        public EntityPropertyInfo(string propertyType, string propertyName, bool? propertyIsNullable = null
            , bool? isEnumPropertyType = false, string propertyDefaultEnumValue = null)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            PropertyIsNullable = propertyIsNullable;
            IsEnumPropertyType = isEnumPropertyType;
            PropertyDefaultEnumValue = propertyDefaultEnumValue;
        }

        /// <summary>
        /// Property type.
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// Property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Property is nullable.
        /// </summary>
        public bool? PropertyIsNullable { get; set; }
        /// <summary>
        /// Property Type is an Enumeration. 
        /// Used in TransformPropertyTypeIfEnumaration
        /// for Many to Many Virtual EntityTypes
        /// </summary>
        public bool? IsEnumPropertyType { get; set; }
        /// <summary>
        /// Property Default Value when using Enumarations
        /// Format will be EnumName.EnumValue
        /// </summary>
        public string PropertyDefaultEnumValue { get; set; }
    }
}