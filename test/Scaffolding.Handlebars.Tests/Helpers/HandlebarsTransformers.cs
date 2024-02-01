using System;
using EntityFrameworkCore.Scaffolding.Handlebars;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Scaffolding.Handlebars.Tests.Helpers
{
    public static class HandlebarsTransformers
    {
        static readonly Dictionary<string, string> _entityTypeNameMappings = new()
        {
            { "Product","ProductRenamed" },
            { "Customer","CustomerRenamed" },
            { "Category", "CategoryRenamed" }
        };

        static readonly Dictionary<string, Func<IEntityType, string, string>> _entityTypeNameMappings2 = new()
        {
            { "Product", (entityType, _) => entityType.GetSchema() + "_ProductRenamed" },
            { "Customer", (entityType, _) => entityType.GetSchema() + "_CustomerRenamed" },
            { "Category", (entityType, _) => entityType.GetSchema() + "_CategoryRenamed" }
        };

        static readonly Dictionary<string, string> _entityPropertyNameMappings = new()
        {
            { "ProductId", "ProductIdRenamed" },
            { "UnitPrice","UnitPriceRenamed"},
            { "CategoryId", "CategoryIdRenamed" },
            { "CategoryName","CategoryNameRenamed" }
        };

        public static string MapEntityName(string entityName) =>
            _entityTypeNameMappings.TryGetValue(entityName, out var nameOverride) ? nameOverride : entityName;

        public static string MapEntityName2(IEntityType entityType, string entityName) =>
            _entityTypeNameMappings2.TryGetValue(entityName, out var nameOverride) ? nameOverride(entityType, entityName) : entityName;

        public static EntityPropertyInfo MapNavPropertyInfo(EntityPropertyInfo e) =>
            new(MapPropertyTypeName(e.PropertyType), MapPropertyName(e.PropertyName));

        public static EntityPropertyInfo MapNavPropertyInfo2(IEntityType entityType, EntityPropertyInfo e) =>
            new(MapEntityName2(entityType, e.PropertyType), MapPropertyName(e.PropertyName));

        public static EntityPropertyInfo MapPropertyInfo(EntityPropertyInfo e) =>
            new(MapPropertyTypeName(e.PropertyType), MapPropertyName(e.PropertyName));

        public static EntityPropertyInfo MapPropertyInfo2(IEntityType entityType, EntityPropertyInfo e) =>
            new(MapEntityName2(entityType, e.PropertyType), MapPropertyName(e.PropertyName));

        private static string MapPropertyTypeName(string propertyTypeName) =>
            _entityTypeNameMappings.TryGetValue(propertyTypeName, out var propertyTypeNameOverride) ? propertyTypeNameOverride : propertyTypeName;

        private static string MapPropertyName(string propertyName) =>
            _entityPropertyNameMappings.TryGetValue(propertyName, out var propertyNameOverride) ? propertyNameOverride : propertyName;
    }
}