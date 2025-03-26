using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Service for transforming entity type definitions.
    /// </summary>
    public interface IEntityTypeTransformationService
    {
        /// <summary>
        /// Transform entity type name.
        /// </summary>
        /// <param name="entityName">Entity type name.</param>
        /// <returns>Transformed entity type name.</returns>
        string TransformTypeEntityName(string entityName);

        /// <summary>
        /// Transform entity type name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="entityName">Entity type name.</param>
        /// <returns>Transformed entity type name.</returns>
        string TransformTypeEntityName(IEntityType entityType, string entityName)
        {
            return TransformTypeEntityName(entityName);
        }

        /// <summary>
        /// Transform entity file name.
        /// </summary>
        /// <param name="entityFileName">Entity file name.</param>
        /// <returns>Transformed entity file name.</returns>
        string TransformEntityFileName(string entityFileName);

        /// <summary>
        /// Transform entity file name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="entityFileName">Entity file name.</param>
        /// <returns>Transformed entity file name.</returns>
        string TransformEntityFileName(IEntityType entityType, string entityFileName)
        {
            return TransformEntityFileName(entityFileName);
        }

        /// <summary>
        /// Transform single property name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Transformed property name.</returns>
        string TransformPropertyName(IEntityType entityType, string propertyName, string propertyType);

        /// <summary>
        /// Transform single navigation property name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Transformed property name.</returns>
        string TransformNavPropertyName(IEntityType entityType, string propertyName, string propertyType);

        /// <summary>
        /// Transforms the Property Type if it is an Enumeration.
        /// Returns null when not an Enumeration
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Transformed property name, null when not an Enumeration</returns>
        public string TransformPropertyTypeIfEnumaration(IEntityType entityType, string propertyName, string propertyType);

        /// <summary>
        /// Transform Default Enum Value for a property
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Default Enumeration Value in format Format will be EnumName.EnumValue</returns>
        public string TransformPropertyDefaultEnum(IEntityType entityType, string propertyName, string propertyType);

        /// <summary>
        /// Transform entity type constructor.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="lines">Constructor lines.</param>
        /// <returns>Transformed constructor lines.</returns>
        List<Dictionary<string, object>> TransformConstructor(IEntityType entityType, List<Dictionary<string, object>> lines);

        /// <summary>
        /// Transform entity type properties.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="properties">Entity type properties.</param>
        /// <returns>Transformed entity type properties.</returns>
        List<Dictionary<string, object>> TransformProperties(IEntityType entityType, List<Dictionary<string, object>> properties);

        /// <summary>
        /// Transform entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="navProperties">Entity type navigation properties.</param>
        /// <returns>Transformed entity type navigation properties.</returns>
        /// 
        List<Dictionary<string, object>> TransformNavigationProperties(IEntityType entityType, List<Dictionary<string, object>> navProperties);
    }
}