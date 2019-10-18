using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Extension methods for Entity Framework <see cref="IEntityType"/> class.
    /// </summary>
    public static class IEntityTypeExtensions
    {
        /// <summary>
        /// Gets all entity type navigations that are to be used in scaffolding.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an IModel.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of navigations for scaffolding.</returns>
        public static IEnumerable<INavigation> GetScaffoldNavigations(this IEntityType entityType, HandlebarsScaffoldingOptions options)
        {
            var navigations = entityType.GetNavigations();
            if (options.ExcludedTables != null && options.ExcludedTables.Any())
            {
                var excludedTables = options.ExcludedTables.Select(t => new TableAndSchema(t)).ToList();
                navigations = navigations.Where(n =>
                        !excludedTables.Any(e =>
                            (string.IsNullOrEmpty(e.Schema) || 
                             string.Equals(n.ForeignKey.GetRelatedEntityType(n.DeclaringEntityType).GetSchema(), e.Schema, StringComparison.OrdinalIgnoreCase))
                            && string.Equals(n.ForeignKey.GetRelatedEntityType(n.DeclaringEntityType).GetTableName(), e.Table, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
            }

            return navigations;
        }

        /// <summary>
        /// Gets all entity type foreign keys that are to be used in scaffolding.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an IModel.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of foreign keys for scaffolding.</returns>
        public static IEnumerable<IForeignKey> GetScaffoldForeignKeys(this IEntityType entityType, HandlebarsScaffoldingOptions options)
        {
            var foreignKeys = entityType.GetForeignKeys();
            if (options.ExcludedTables != null && options.ExcludedTables.Any())
            {
                var excludedTables = options.ExcludedTables.Select(t => new TableAndSchema(t)).ToList();
                foreignKeys = foreignKeys.Where(f =>
                        !excludedTables.Any(e =>
                            (string.IsNullOrEmpty(e.Schema) ||
                             string.Equals(f.GetRelatedEntityType(f.DeclaringEntityType).GetSchema(), e.Schema, StringComparison.OrdinalIgnoreCase))
                            && string.Equals(f.GetRelatedEntityType(f.DeclaringEntityType).GetTableName(), e.Table, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
            }

            return foreignKeys;
        }
    }
}
