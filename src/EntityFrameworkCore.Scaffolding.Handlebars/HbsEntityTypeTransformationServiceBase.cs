using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Default service for transforming entity type definitions.
    /// </summary>
    public abstract class HbsEntityTypeTransformationServiceBase : IEntityTypeTransformationService
    {
        /// <summary>
        /// Entity name transformer.
        /// </summary>
        protected Func<string, string> EntityTypeNameTransformer { get; }

        /// <summary>
        /// Entity file name transformer.
        /// </summary>
        protected Func<string, string> EntityFileNameTransformer { get; }

        /// <summary>
        /// Constructor transformer.
        /// </summary>
        protected Func<EntityPropertyInfo, EntityPropertyInfo> ConstructorTransformer { get; set; }

        /// <summary>
        /// Property name transformer.
        /// </summary>
        protected Func<EntityPropertyInfo, EntityPropertyInfo> PropertyTransformer { get; set; }

        /// <summary>
        /// Navigation property name transformer.
        /// </summary>
        protected Func<EntityPropertyInfo, EntityPropertyInfo> NavPropertyTransformer { get; set; }

        /// <summary>
        /// Entity name transformer.
        /// </summary>
        protected Func<IEntityType, string, string> EntityTypeNameTransformer2 { get; set; }

        /// <summary>
        /// Entity file name transformer.
        /// </summary>
        protected Func<IEntityType, string, string> EntityFileNameTransformer2 { get; set; }

        /// <summary>
        /// Constructor transformer.
        /// </summary>
        protected Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> ConstructorTransformer2 { get; set; }

        /// <summary>
        /// Property name transformer.
        /// </summary>
        protected Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> PropertyTransformer2 { get; set; }

        /// <summary>
        /// Navigation property name transformer.
        /// </summary>
        protected Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> NavPropertyTransformer2 { get; set; }

        /// <summary>
        /// HbsEntityTypeTransformationService constructor.
        /// </summary>
        /// <param name="entityTypeNameTransformer">Entity type name transformer.</param>
        /// <param name="entityFileNameTransformer">Entity file name transformer.</param>
        public HbsEntityTypeTransformationServiceBase(
            Func<string, string> entityTypeNameTransformer = null,
            Func<string, string> entityFileNameTransformer = null)
        {
            EntityTypeNameTransformer = entityTypeNameTransformer;
            EntityFileNameTransformer = entityFileNameTransformer;
        }

        /// <summary>
        /// Transform entity type name.
        /// </summary>
        /// <param name="entityName">Entity type name.</param>
        /// <returns>Transformed entity type name.</returns>
        public string TransformTypeEntityName(string entityName) =>
            EntityTypeNameTransformer?.Invoke(entityName) ?? entityName;

        /// <summary>
        /// Transform entity type name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="entityName">Entity type name.</param>
        /// <returns>Transformed entity type name.</returns>
        public string TransformTypeEntityName(IEntityType entityType, string entityName) =>
            EntityTypeNameTransformer2?.Invoke(entityType, entityName)
            ?? EntityTypeNameTransformer?.Invoke(entityName)
            ?? entityName;

        /// <summary>
        /// Transform entity file name.
        /// </summary>
        /// <param name="entityFileName">Entity file name.</param>
        /// <returns>Transformed entity file name.</returns>
        public string TransformEntityFileName(string entityFileName) =>
            EntityFileNameTransformer?.Invoke(entityFileName) ?? entityFileName;

        /// <summary>
        /// Transform entity file name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="entityFileName">Entity file name.</param>
        /// <returns>Transformed entity file name.</returns>
        public string TransformEntityFileName(IEntityType entityType, string entityFileName) => 
            EntityFileNameTransformer2?.Invoke(entityType, entityFileName) 
            ?? EntityFileNameTransformer?.Invoke(entityFileName) 
            ?? entityFileName;

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
            if (PropertyTransformer2 != null)
                return PropertyTransformer2?.Invoke(entityType, propTypeInfo)?.PropertyName;
            else if (PropertyTransformer != null)
                return PropertyTransformer?.Invoke(propTypeInfo)?.PropertyName;
            else
                return propertyName;
        }

        /// <summary>
        /// Transforms the Property Type if it is an Enumeration.
        /// Returns null when not an Enumeration
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Transformed property name, null when not an Enumeration</returns>
        public string TransformPropertyTypeIfEnumaration(IEntityType entityType, string propertyName, string propertyType)
        {
            var propTypeInfo = new EntityPropertyInfo { PropertyName = propertyName, PropertyType = propertyType };
            if (PropertyTransformer2 != null)
            {
                EntityPropertyInfo epi = PropertyTransformer2?.Invoke(entityType, propTypeInfo);
                return epi.IsEnumPropertyType == true ? epi.PropertyType : null;
            }
            else if (PropertyTransformer != null)
            {
                EntityPropertyInfo epi = PropertyTransformer?.Invoke(propTypeInfo);
                return epi.IsEnumPropertyType == true ? epi.PropertyType : null;
            }
            return null;
        }

        /// <summary>
        /// Transform Default Enum Value for a property
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyType">Property type</param>
        /// <returns>Default Enumeration Value in format Format will be EnumName.EnumValue</returns>
        public string TransformPropertyDefaultEnum(IEntityType entityType, string propertyName, string propertyType)
        {
            var propTypeInfo = new EntityPropertyInfo { PropertyName = propertyName, PropertyType = propertyType };
            if (PropertyTransformer2 != null)
                return PropertyTransformer2?.Invoke(entityType, propTypeInfo)?.PropertyDefaultEnumValue;
            else if (PropertyTransformer != null)
                return PropertyTransformer?.Invoke(propTypeInfo)?.PropertyDefaultEnumValue;
            else
                return null;
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
            if (NavPropertyTransformer2 != null)
                return NavPropertyTransformer2?.Invoke(entityType, propTypeInfo)?.PropertyName;
            else if (NavPropertyTransformer != null)
                return NavPropertyTransformer?.Invoke(propTypeInfo)?.PropertyName;
            else
                return propertyName;
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
                EntityPropertyInfo transformedProp;
                if (ConstructorTransformer2 != null)
                    transformedProp = ConstructorTransformer2?.Invoke(entityType, propTypeInfo);
                else if (ConstructorTransformer != null)
                    transformedProp = ConstructorTransformer?.Invoke(propTypeInfo);
                else
                    transformedProp = propTypeInfo;

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
                    (property["property-isnullable"] as bool?) == true,
                    (property["property-isenum"] as bool?) == true,
                    property["property-default-enum"] as string);
                EntityPropertyInfo transformedProp;
                if (PropertyTransformer2 != null)
                    transformedProp = PropertyTransformer2?.Invoke(entityType, propTypeInfo);
                else if (PropertyTransformer != null)
                    transformedProp = PropertyTransformer?.Invoke(propTypeInfo);
                else
                    transformedProp = propTypeInfo;
                var propertyIsNullable = transformedProp.PropertyIsNullable != null
                    ? transformedProp.PropertyIsNullable
                    : (bool)property["property-isnullable"];
                var isEnumPropertyType = transformedProp.IsEnumPropertyType != null
                    ? transformedProp.IsEnumPropertyType
                    : (bool)property["property-isenum"];
                string propertyDefaultEnumValue = transformedProp.PropertyDefaultEnumValue != null
                    ? transformedProp.PropertyDefaultEnumValue
                    : property["property-default-enum"] as string;

                transformedProperties.Add(new Dictionary<string, object>
                {
                    { "property-type", transformedProp.PropertyType },
                    { "property-name", transformedProp.PropertyName },
                    { "property-annotations", property["property-annotations"] },
                    { "property-comment", property["property-comment"] },
                    { "property-isnullable", propertyIsNullable },
                    { "property-isenum", isEnumPropertyType },
                    { "property-default-enum", propertyDefaultEnumValue },
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
        /// 
        public List<Dictionary<string, object>> TransformNavigationProperties(IEntityType entityType, List<Dictionary<string, object>> navProperties)
        {
            var transformedNavProperties = new List<Dictionary<string, object>>();

            foreach (var navProperty in navProperties)
            {
                var propTypeInfo = new EntityPropertyInfo(navProperty["nav-property-type"] as string,
                    navProperty["nav-property-name"] as string);
                EntityPropertyInfo transformedProp;
                if (NavPropertyTransformer2 != null)
                    transformedProp = NavPropertyTransformer2?.Invoke(entityType, propTypeInfo);
                else if (NavPropertyTransformer != null)
                    transformedProp = NavPropertyTransformer?.Invoke(propTypeInfo);
                else
                    transformedProp = propTypeInfo;

                if (navProperty["nav-property-isnullable"] is bool propertyIsNullable
                    && propertyIsNullable && !transformedProp.PropertyType.EndsWith("?"))
                {
                    transformedProp.PropertyType += "?";
                }

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