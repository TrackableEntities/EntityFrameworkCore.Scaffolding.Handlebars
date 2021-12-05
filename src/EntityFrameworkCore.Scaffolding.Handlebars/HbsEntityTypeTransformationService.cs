using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Default service for transforming entity type definitions.
    /// </summary>
    public class HbsEntityTypeTransformationService : IEntityTypeTransformationService
    {    
        /// <summary>
        /// Entity name transformer.
        /// </summary>
        public Func<string, string> EntityTypeNameTransformer { get; }

        /// <summary>
        /// Entity file name transformer.
        /// </summary>
        public Func<string, string> EntityFileNameTransformer { get; }

        /// <summary>
        /// Constructor transformer.
        /// </summary>
        public Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> ConstructorTransformer { get; }

        /// <summary>
        /// Property name transformer.
        /// </summary>
        public Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> PropertyTransformer { get; }

        /// <summary>
        /// Navigation property name transformer.
        /// </summary>
        public Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> NavPropertyTransformer { get; }

        /// <summary>
        /// HbsEntityTypeTransformationService constructor.
        /// </summary>
        /// <param name="entityTypeNameTransformer">Entity type name transformer.</param>
        /// <param name="entityFileNameTransformer">Entity file name transformer.</param>
        /// <param name="constructorTransformer">Constructor transformer.</param>
        /// <param name="propertyTransformer">Property name transformer.</param>
        /// <param name="navPropertyTransformer">Navigation property name transformer.</param>
        public HbsEntityTypeTransformationService(
            Func<string, string> entityTypeNameTransformer = null,
            Func<string, string> entityFileNameTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> constructorTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> propertyTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> navPropertyTransformer = null)
        {
            EntityTypeNameTransformer = entityTypeNameTransformer;
            EntityFileNameTransformer = entityFileNameTransformer;
            ConstructorTransformer = constructorTransformer;
            PropertyTransformer = propertyTransformer;
            NavPropertyTransformer = navPropertyTransformer;
        }

        /// <summary>
        /// Transform entity type name.
        /// </summary>
        /// <param name="entityName">Entity type name.</param>
        /// <returns>Transformed entity type name.</returns>
        public string TransformTypeEntityName(string entityName) =>
            EntityTypeNameTransformer?.Invoke(entityName) ?? entityName;

        /// <summary>
        /// Transform entity file name.
        /// </summary>
        /// <param name="entityFileName">Entity file name.</param>
        /// <returns>Transformed entity file name.</returns>
        public string TransformEntityFileName(string entityFileName) =>
            EntityFileNameTransformer?.Invoke(entityFileName) ?? entityFileName;

        /// <summary>
        /// Transform single property name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Transformed property name.</returns>
        public string TransformPropertyName(IEntityType entityType, string propertyName, string propertyType)
        {
            var propTypeInfo = new EntityPropertyInfo { PropertyName = propertyName, PropertyType = propertyType };
            return PropertyTransformer?.Invoke(entityType, propTypeInfo)?.PropertyName ?? propertyName;
        }

        /// <summary>
        /// Transform single navigation property name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Transformed property name.</returns>
        public string TransformNavPropertyName(IEntityType entityType, string propertyName, string propertyType)
        {
            var propTypeInfo = new EntityPropertyInfo { PropertyName = propertyName, PropertyType = propertyType };
            return NavPropertyTransformer?.Invoke(entityType, propTypeInfo)?.PropertyName ?? propertyName;
        }

        /// <summary>
        /// Transform entity type constructor.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="lines">Constructor lines.</param>
        /// <returns>Transformed constructor lines.</returns>
        public List<Dictionary<string, object>> TransformConstructor(IEntityType entityType, List<Dictionary<string, object>> lines)
        {
            var transformedLines = new List<Dictionary<string, object>>();

            foreach (var line in lines)
            {
                var propTypeInfo = new EntityPropertyInfo(line["property-type"] as string,
                    line["property-name"] as string);
                var transformedProp = ConstructorTransformer?.Invoke(entityType, propTypeInfo) ?? propTypeInfo;

                transformedLines.Add(new Dictionary<string, object>
                {
                    { "property-name", transformedProp.PropertyName },
                    { "property-type", transformedProp.PropertyType }
                });
            }

            return transformedLines;
        }

        /// <summary>
        /// Transform entity type properties.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="properties">Entity type properties.</param>
        /// <returns>Transformed entity type properties.</returns>
        public List<Dictionary<string, object>> TransformProperties(IEntityType entityType, List<Dictionary<string, object>> properties)
        {
            var transformedProperties = new List<Dictionary<string, object>>();

            foreach (var property in properties)
            {
                var propTypeInfo = new EntityPropertyInfo(property["property-type"] as string, 
                    property["property-name"] as string,
                    (property["property-isnullable"] as bool?) == true);
                var transformedProp = PropertyTransformer?.Invoke(entityType, propTypeInfo) ?? propTypeInfo;

                transformedProperties.Add(new Dictionary<string, object>
                {
                    { "property-type", transformedProp.PropertyType },
                    { "property-name", transformedProp.PropertyName },
                    { "property-annotations", property["property-annotations"] },
                    { "property-comment", property["property-comment"] },
                    { "property-isnullable", transformedProp.PropertyIsNullable },
                    { "nullable-reference-types", property["nullable-reference-types"] }
                });
            }

            return transformedProperties;
        }

        /// <summary>
        /// Transform entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="navProperties">Entity type navigation properties.</param>
        /// <returns>Transformed entity type navigation properties.</returns>
        public List<Dictionary<string, object>> TransformNavigationProperties(IEntityType entityType, List<Dictionary<string, object>> navProperties)
        {
            var transformedNavProperties = new List<Dictionary<string, object>>();

            foreach (var navProperty in navProperties)
            {
                var propTypeInfo = new EntityPropertyInfo(navProperty["nav-property-type"] as string,
                    navProperty["nav-property-name"] as string);
                var transformedProp = NavPropertyTransformer?.Invoke(entityType, propTypeInfo) ?? propTypeInfo;

                var newNavProperty = new Dictionary<string, object>(navProperty)
                {
                    ["nav-property-type"] = transformedProp.PropertyType,
                    ["nav-property-name"] = transformedProp.PropertyName
                };

                transformedNavProperties.Add(newNavProperty);
            }

            return transformedNavProperties;
        }
    }
}