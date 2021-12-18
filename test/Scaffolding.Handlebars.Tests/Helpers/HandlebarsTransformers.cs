using EntityFrameworkCore.Scaffolding.Handlebars;
using System.Collections.Generic;

namespace Scaffolding.Handlebars.Tests.Helpers
{
    public static class HandlebarsTransformers
    {
        static readonly Dictionary<string, string> _entityTypeNameMappings = new(){
            {"Product","ProductRenamed"},
            {"Category","CategoryRenamed"}
        };
        static readonly Dictionary<string, string> _entityPropertyNameMappings = new(){
            {"UnitPrice","UnitPriceRenamed"},
            {"CategoryName","CategoryNameRenamed"}
        };
        public static string MapEntityName(string entityName) =>
            _entityTypeNameMappings.TryGetValue(entityName, out var nameOverride) ? nameOverride : entityName;

        public static EntityPropertyInfo MapNavPropertyInfo(EntityPropertyInfo e) =>
            new(MapPropertyTypeName(e.PropertyType), MapPropertyName(e.PropertyName));

        public static EntityPropertyInfo MapPropertyInfo(EntityPropertyInfo e) =>
            new(MapPropertyTypeName(e.PropertyType), MapPropertyName(e.PropertyName));

        private static string MapPropertyTypeName(string propertyTypeName) =>
            _entityTypeNameMappings.TryGetValue(propertyTypeName, out var propertyTypeNameOverride) ? propertyTypeNameOverride : propertyTypeName;

        private static string MapPropertyName(string propertyName) =>
            _entityPropertyNameMappings.TryGetValue(propertyName, out var propertyNameOverride) ? propertyNameOverride : propertyName;
    }
}