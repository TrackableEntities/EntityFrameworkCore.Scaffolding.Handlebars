using System.Linq;
using System.Collections.Generic;

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
            return entityType
                .GetNavigations()
                .Where(nav => !Helpers.TableExcluder.IsExcluded(options, nav.TargetEntityType))
                .ToList();
        }

        /// <summary>
        /// Gets all entity type skip navigations that are to be used in scaffolding.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an IModel.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of navigations for scaffolding.</returns>
        public static IEnumerable<ISkipNavigation> GetScaffoldSkipNavigations(this IEntityType entityType, HandlebarsScaffoldingOptions options)
        {
            return entityType
                .GetSkipNavigations()
                .Where(nav => !Helpers.TableExcluder.IsExcluded(options, nav.TargetEntityType))
                .ToList();
        }

        /// <summary>
        /// Gets all entity type foreign keys that are to be used in scaffolding.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an IModel.</param>
        /// <param name="options">Scaffolding options to use in determining entity types.</param>
        /// <returns>Filtered set of foreign keys for scaffolding.</returns>
        public static IEnumerable<IForeignKey> GetScaffoldForeignKeys(this IEntityType entityType, HandlebarsScaffoldingOptions options)
        {
            return entityType
                .GetForeignKeys()
                .Where(fk => !Helpers.TableExcluder.IsExcluded(options, fk.GetRelatedEntityType(fk.DeclaringEntityType)))
                .ToList();
        }
    }
}
