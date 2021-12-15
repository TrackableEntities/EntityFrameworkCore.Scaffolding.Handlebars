using EntityFrameworkCore.Scaffolding.Handlebars;
using System.Collections.Generic;

namespace Scaffolding.Handlebars.Tests.Helpers
{
    public static class HandlebarsTransformers
    {
        static Dictionary<string, string> entityTypeNameMappings = new Dictionary<string, string>(){
            {"Product","ProductRenamed"},
            {"Category","CategoryRenamed"}
        };
        static Dictionary<string, string> entityPropertyNameMappings = new Dictionary<string, string>(){
            {"UnitPrice","UnitPriceRenamed"},
            {"CategoryName","CategoryNameRenamed"}
        };
        public static string MapEntityName(string entityName)
        {
            var nameOverride = string.Empty;
            if (entityTypeNameMappings.TryGetValue(entityName, out nameOverride)) return nameOverride;
            return entityName;
        }
        public static EntityPropertyInfo MapNavPropertyInfo(EntityPropertyInfo e)
        {
            return new EntityPropertyInfo(MapPropertyTypeName(e.PropertyType), MapPropertyName(e.PropertyName));
        }
        public static EntityPropertyInfo MapPropertyInfo(EntityPropertyInfo e)
        {
            return new EntityPropertyInfo(MapPropertyTypeName(e.PropertyType), MapPropertyName(e.PropertyName));
        }
        private static string MapPropertyTypeName(string propertyTypeName)
        {
            var propertyTypeNameOverride = string.Empty;
            if (entityTypeNameMappings.TryGetValue(propertyTypeName, out propertyTypeNameOverride)) return propertyTypeNameOverride;
            return propertyTypeName;
        }
        private static string MapPropertyName(string propertyName)
        {
            var propertyNameOverride = string.Empty;
            if (entityPropertyNameMappings.TryGetValue(propertyName, out propertyNameOverride)) return propertyNameOverride;
            return propertyName;
        }
    }
}