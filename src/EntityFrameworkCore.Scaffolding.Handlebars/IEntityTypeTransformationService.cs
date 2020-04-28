using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Service for transforming entity type definitions.
    /// </summary>
    public interface IEntityTypeTransformationService
    {
        /// <summary>
        /// Transform entity type name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <returns>Transformed entity type name.</returns>
        string TransformEntityName(IEntityType entityType);

        /// <summary>
        /// Transform entity file name.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <returns>Transformed entity file name.</returns>
        string TransformEntityFileName(IEntityType entityType);

        /// <summary>
        /// Transform single property name.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Transformed property name.</returns>
        string TransformPropertyName(string propertyName);

        /// <summary>
        /// Transform entity type constructor.
        /// </summary>
        /// <param name="lines">Constructor lines.</param>
        /// <returns>Transformed constructor lines.</returns>
        List<Dictionary<string, object>> TransformConstructor(List<Dictionary<string, object>> lines);

        /// <summary>
        /// Transform entity type properties.
        /// </summary>
        /// <param name="properties">Entity type properties.</param>
        /// <returns>Transformed entity type properties.</returns>
        List<Dictionary<string, object>> TransformProperties(List<Dictionary<string, object>> properties);

        /// <summary>
        /// Transform entity type navigation properties.
        /// </summary>
        /// <param name="navProperties">Entity type navigation properties.</param>
        /// <returns>Transformed entity type navigation properties.</returns>
        List<Dictionary<string, object>> TransformNavigationProperties(List<Dictionary<string, object>> navProperties);
    }
}