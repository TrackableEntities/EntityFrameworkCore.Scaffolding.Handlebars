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
        public EntityPropertyInfo(string propertyType, string propertyName, bool? propertyIsNullable = null)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            PropertyIsNullable = propertyIsNullable;
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
    }
}