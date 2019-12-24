using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// DbContext generator that does not generate any code.
    /// </summary>
    public class NullCSharpDbContextGenerator : ICSharpDbContextGenerator
    {
        /// <summary>
        /// Generate the DbContext class.
        /// </summary>
        /// <param name="model">Metadata about the shape of entities, the relationships between them, and how they map to the database.</param>
        /// <param name="namespace">DbContext namespace.</param>
        /// <param name="contextName">Name of DbContext class.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="useDataAnnotations">If false use fluent modeling API.</param>
        /// <param name="suppressConnectionStringWarning">True to suppress connection string warning.</param>
        /// <returns>Generated DbContext class.</returns>
        public string WriteCode(IModel model, string contextName, string connectionString, string contextNamespace, string modelNamespace, bool useDataAnnotations, bool suppressConnectionStringWarning)
        {
            return string.Empty;
        }
    }
}