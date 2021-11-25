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
            var result = new List<INavigation>();
            var navigations = entityType.GetNavigations();
            var excludedTables = GetExcludedTables(options);

            foreach (var nav in navigations)
            {
                var schema = nav.TargetEntityType.GetSchema();
                var table = nav.TargetEntityType.GetTableName();
                if (!excludedTables.Any(e =>
                    (!options.EnableSchemaFolders && string.Equals(table, e.Table, StringComparison.OrdinalIgnoreCase))
                    || string.Equals(schema, e.Schema, StringComparison.OrdinalIgnoreCase)
                       && string.Equals(table, e.Table, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(nav);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all entity type skip navigations that are to be used in scaffolding.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an IModel.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of navigations for scaffolding.</returns>
        public static IEnumerable<ISkipNavigation> GetScaffoldSkipNavigations(this IEntityType entityType, HandlebarsScaffoldingOptions options)
        {
            var result = new List<ISkipNavigation>();
            var navigations = entityType.GetSkipNavigations();
            var excludedTables = GetExcludedTables(options);

            foreach (var nav in navigations)
            {
                var schema = nav.TargetEntityType.GetSchema();
                var table = nav.TargetEntityType.GetTableName();
                if (!excludedTables.Any(e =>
                    (!options.EnableSchemaFolders && string.Equals(table, e.Table, StringComparison.OrdinalIgnoreCase))
                    || string.Equals(schema, e.Schema, StringComparison.OrdinalIgnoreCase)
                       && string.Equals(table, e.Table, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(nav);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all entity type foreign keys that are to be used in scaffolding.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an IModel.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of foreign keys for scaffolding.</returns>
        public static IEnumerable<IForeignKey> GetScaffoldForeignKeys(this IEntityType entityType, HandlebarsScaffoldingOptions options)
        {
            var result = new List<IForeignKey>();
            var foreignKeys = entityType.GetForeignKeys();
            var excludedTables = GetExcludedTables(options);

            foreach (var fk in foreignKeys)
            {
                var schema = fk.GetRelatedEntityType(fk.DeclaringEntityType).GetSchema();
                var table = fk.GetRelatedEntityType(fk.DeclaringEntityType).GetTableName();
                if (!excludedTables.Any(e =>
                    (!options.EnableSchemaFolders && string.Equals(table, e.Table, StringComparison.OrdinalIgnoreCase))
                    || string.Equals(schema, e.Schema, StringComparison.OrdinalIgnoreCase)
                       && string.Equals(table, e.Table, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(fk);
                }
            }
            return result;
        }

        private static List<TableAndSchema> GetExcludedTables(HandlebarsScaffoldingOptions options)
        {
            var excludedTables = new List<TableAndSchema>();
            if (options.ExcludedTables != null)
                excludedTables = options.ExcludedTables.Select(t => new TableAndSchema(t)).ToList();
            return excludedTables;
        }
    }
}
