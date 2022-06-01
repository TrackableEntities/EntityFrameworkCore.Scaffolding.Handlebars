using System.Linq;
using System.Collections.Generic;

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
            return model
                .GetEntityTypes()
                .Where(t => !Helpers.TableExcluder.IsExcluded(options, t))
                .ToList();
        }
    }
}
