﻿using Microsoft.EntityFrameworkCore;
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
        public Func<IEntityType, string> EntityNameTransformer { get; }

        /// <summary>
        /// Entity file name transformer.
        /// </summary>
        public Func<IEntityType, string> EntityFileNameTransformer { get; }

        /// <summary>
        /// Constructor transformer.
        /// </summary>
        public Func<EntityPropertyInfo, EntityPropertyInfo> ConstructorTransformer { get; }

        /// <summary>
        /// Property name transformer.
        /// </summary>
        public Func<EntityPropertyInfo, EntityPropertyInfo> PropertyTransformer { get; }

        /// <summary>
        /// Navigation property name transformer.
        /// </summary>
        public Func<EntityPropertyInfo, EntityPropertyInfo> NavPropertyTransformer { get; }

        /// <summary>
        /// HbsEntityTypeTransformationService constructor.
        /// </summary>
        /// <param name="entityNameTransformer">Entity name transformer.</param>
        /// <param name="entityFileNameTransformer">Entity file name transformer.</param>
        /// <param name="constructorTransformer">Constructor transformer.</param>
        /// <param name="propertyTransformer">Property name transformer.</param>
        /// <param name="navPropertyTransformer">Navigation property name transformer.</param>
        public HbsEntityTypeTransformationService(
            Func<IEntityType, string> entityNameTransformer = null,
            Func<IEntityType, string> entityFileNameTransformer = null,
            Func<EntityPropertyInfo, EntityPropertyInfo> constructorTransformer = null,
            Func<EntityPropertyInfo, EntityPropertyInfo> propertyTransformer = null,
            Func<EntityPropertyInfo, EntityPropertyInfo> navPropertyTransformer = null)
        {
            EntityNameTransformer = entityNameTransformer;
            EntityFileNameTransformer = entityFileNameTransformer;
            ConstructorTransformer = constructorTransformer;
            PropertyTransformer = propertyTransformer;
            NavPropertyTransformer = navPropertyTransformer;
        }

        /// <summary>
        /// Transform entity type name.
        /// (default is .Name)
        /// </summary>
        /// <param name="entity">Entity type.</param>
        /// <returns>Transformed entity type name.</returns>
        public string TransformEntityName(IEntityType entity) =>
            EntityNameTransformer?.Invoke(entity) ?? entity.Name;

        /// <summary>
        /// Transform entity file name.
        /// (default is .DisplayName() from Microsoft.EntityFrameworkCore Extension)
        /// </summary>
        /// <param name="entity">Entity Type.</param>
        /// <returns>Transformed entity file name.</returns>
        public string TransformEntityFileName(IEntityType entity) =>
            EntityFileNameTransformer?.Invoke(entity) ?? entity.DisplayName();

        /// <summary>
        /// Transform single property name.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Transformed property name.</returns>
        public string TransformPropertyName(string propertyName)
        {
            var propTypeInfo = new EntityPropertyInfo { PropertyName = propertyName };
            return PropertyTransformer?.Invoke(propTypeInfo)?.PropertyName ?? propertyName;
        }

        /// <summary>
        /// Transform entity type constructor.
        /// </summary>
        /// <param name="lines">Constructor lines.</param>
        /// <returns>Transformed constructor lines.</returns>
        public List<Dictionary<string, object>> TransformConstructor(List<Dictionary<string, object>> lines)
        {
            var transformedLines = new List<Dictionary<string, object>>();

            foreach (var line in lines)
            {
                var propTypeInfo = new EntityPropertyInfo(line["property-type"] as string,
                    line["property-name"] as string,
                    line["property-isnullable"] as bool?
                    );
                var transformedProp = ConstructorTransformer?.Invoke(propTypeInfo) ?? propTypeInfo;

                transformedLines.Add(new Dictionary<string, object>
                {
                    { "property-name", transformedProp.PropertyName },
                    { "property-type", transformedProp.PropertyType },
                    { "property-isnullable", transformedProp.PropertyIsNullable }
                });
            }

            return transformedLines;
        }

        /// <summary>
        /// Transform entity type properties.
        /// </summary>
        /// <param name="properties">Entity type properties.</param>
        /// <returns>Transformed entity type properties.</returns>
        public List<Dictionary<string, object>> TransformProperties(List<Dictionary<string, object>> properties)
        {
            var transformedProperties = new List<Dictionary<string, object>>();

            foreach (var property in properties)
            {
                var propTypeInfo = new EntityPropertyInfo(property["property-type"] as string, 
                    property["property-name"] as string,
                    property["property-isnullable"] as bool?);
                var transformedProp = PropertyTransformer?.Invoke(propTypeInfo) ?? propTypeInfo;

                transformedProperties.Add(new Dictionary<string, object>
                {
                    { "property-type", transformedProp.PropertyType },
                    { "property-name", transformedProp.PropertyName },
                    { "property-annotations", property["property-annotations"] },
                    { "property-isnullable", transformedProp.PropertyIsNullable },
                });
            }

            return transformedProperties;
        }

        /// <summary>
        /// Transform entity type navigation properties.
        /// </summary>
        /// <param name="navProperties">Entity type navigation properties.</param>
        /// <returns>Transformed entity type navigation properties.</returns>
        public List<Dictionary<string, object>> TransformNavigationProperties(List<Dictionary<string, object>> navProperties)
        {
            var transformedNavProperties = new List<Dictionary<string, object>>();

            foreach (var navProperty in navProperties)
            {
                var propTypeInfo = new EntityPropertyInfo(navProperty["nav-property-type"] as string,
                    navProperty["nav-property-name"] as string);
                var transformedProp = NavPropertyTransformer?.Invoke(propTypeInfo) ?? propTypeInfo;

                transformedNavProperties.Add(new Dictionary<string, object>
                {
                    { "nav-property-collection", navProperty["nav-property-collection"] },
                    { "nav-property-type", transformedProp.PropertyType },
                    { "nav-property-name", transformedProp.PropertyName },
                    { "nav-property-annotations", navProperty["nav-property-annotations"] }
                });
            }

            return transformedNavProperties;
        }
    }
}