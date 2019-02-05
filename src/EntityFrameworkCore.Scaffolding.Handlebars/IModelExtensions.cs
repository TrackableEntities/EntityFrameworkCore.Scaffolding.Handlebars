using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Extension methods for Entity Framework <see cref="IModel"/> class.
    /// </summary>
    public static class IModelExtensions
    {
        /// <summary>
        /// Gets all entity types that are to be used in scaffolding.
        /// </summary>
        /// <param name="model">Model to retrieve entity types from.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of entity types for scaffolding.</returns>
        public static IEnumerable<IEntityType> GetScaffoldEntityTypes(this IModel model, HandlebarsScaffoldingOptions options)
        {
            var entityTypes = model.GetEntityTypes();
            if (options.ExcludedTables != null && options.ExcludedTables.Any())
            {
                var excludedTables = options.ExcludedTables.Select(t => new TableAndSchema(t)).ToList();
                entityTypes = entityTypes.Where(t =>
                        !excludedTables.Any(e =>
                            (string.IsNullOrEmpty(e.Schema) || 
                             string.Equals(t.Relational().Schema, e.Schema, StringComparison.OrdinalIgnoreCase))
                            && string.Equals(t.Relational().TableName, e.Table, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
            }

            return entityTypes;
        }
    }
}
