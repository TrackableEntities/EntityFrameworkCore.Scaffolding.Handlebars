using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal;

/// <summary>
/// Generator for entity type classes.
/// </summary>
public interface ICSharpEntityTypeGenerator
{
    /// <summary>
    /// Generate entity type class.
    /// </summary>
    /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
    /// <param name="namespace">Entity type namespace.</param>
    /// <param name="useDataAnnotations">If true use data annotations.</param>
    /// <param name="useNullableReferenceTypes">If true use nullable reference types.</param>
    /// <returns>Generated entity type.</returns>
    string WriteCode(
        IEntityType entityType,
        string @namespace,
        bool useDataAnnotations,
        bool useNullableReferenceTypes);
}