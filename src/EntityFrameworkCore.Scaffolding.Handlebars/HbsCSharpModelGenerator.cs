using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Scaffolding generator for DbContext and entity type classes using Handlebars templates.
    /// </summary>
    public class HbsCSharpModelGenerator : ModelCodeGenerator
    {
        /// <summary>
        /// Constructor for the HbsCSharpModelGenerator.
        /// </summary>
        /// <param name="dependencies">Service dependencies parameter class for HbsCSharpModelGenerator.</param>
        public HbsCSharpModelGenerator(ModelCodeGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        /// <summary>Generates code for a model.</summary>
        /// <param name="model"> The model. </param>
        /// <param name="namespace"> The namespace. </param>
        /// <param name="contextDir"> The directory of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="contextName"> The name of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="connectionString"> The connection string. </param>
        /// <param name="options"> The options to use during generation. </param>
        /// <returns> The generated model. </returns>
        public override ScaffoldedModel GenerateModel(IModel model, string @namespace, string contextDir, string contextName,
            string connectionString, ModelCodeGenerationOptions options)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the programming language supported by this service.
        /// </summary>
        public override string Language { get; }
    }
}