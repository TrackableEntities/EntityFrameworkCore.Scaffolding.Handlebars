using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal;

/// <summary>
/// Generator for the DbContext class.
/// </summary>
public interface ICSharpDbContextGenerator
{
    /// <summary>
    /// Generate the DbContext class.
    /// </summary>
    /// <param name="model">Metadata about the shape of entities, the relationships between them, and how they map to the database.</param>
    /// <param name="contextName">Name of DbContext class.</param>
    /// <param name="connectionString">Database connection string.</param>
    /// <param name="contextNamespace">Context namespace.</param>
    /// <param name="modelNamespace">Model namespace.</param>
    /// <param name="useDataAnnotations">If false use fluent modeling API.</param>
    /// <param name="useNullableReferenceTypes">True if using nullable reference types.</param>
    /// <param name="suppressConnectionStringWarning">Suppress connection string warning.</param>
    /// <param name="suppressOnConfiguring">Suppress OnConfiguring method.</param>
    /// <returns>DbContext class.</returns>
    string WriteCode(
        IModel model,
        string contextName,
        string connectionString,
        string contextNamespace,
        string modelNamespace,
        bool useDataAnnotations,
        bool useNullableReferenceTypes,
        bool suppressConnectionStringWarning,
        bool suppressOnConfiguring);
}