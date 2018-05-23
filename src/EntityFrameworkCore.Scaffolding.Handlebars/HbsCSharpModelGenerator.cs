// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System.IO;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Scaffolding generator for DbContext and entity type classes using Handlebars templates.
    /// </summary>
    public class HbsCSharpModelGenerator : ModelCodeGenerator
    {
        private const string FileExtension = ".cs";

        /// <summary>
        /// DbContext generator.
        /// </summary>
        public virtual ICSharpDbContextGenerator CSharpDbContextGenerator { get; }

        /// <summary>
        /// Entity type generator.
        /// </summary>
        public virtual ICSharpEntityTypeGenerator CSharpEntityTypeGenerator { get; }

        /// <summary>
        /// Constructor for the HbsCSharpModelGenerator.
        /// </summary>
        /// <param name="dependencies">Service dependencies parameter class for HbsCSharpModelGenerator.</param>
        /// <param name="cSharpDbContextGenerator">DbContext generator.</param>
        /// <param name="cSharpEntityTypeGenerator">Entity type generator.</param>
        public HbsCSharpModelGenerator(ModelCodeGeneratorDependencies dependencies,
            ICSharpDbContextGenerator cSharpDbContextGenerator,
            ICSharpEntityTypeGenerator cSharpEntityTypeGenerator) : base(dependencies)
        {
            CSharpDbContextGenerator = cSharpDbContextGenerator;
            CSharpEntityTypeGenerator = cSharpEntityTypeGenerator;
        }

        /// <summary>Generates code for a model.</summary>
        /// <param name="model"> The model. </param>
        /// <param name="namespace"> The namespace. </param>
        /// <param name="contextDir"> The directory of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="contextName"> The name of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="connectionString"> The connection string. </param>
        /// <param name="options"> The options to use during generation. </param>
        /// <returns> The generated model. </returns>
        public override ScaffoldedModel GenerateModel(IModel model,
            string @namespace,
            string contextDir,
            string contextName,
            string connectionString,
            ModelCodeGenerationOptions options)
        {
            var resultingFiles = new ScaffoldedModel();

            var generatedCode = CSharpDbContextGenerator.WriteCode(model, @namespace, contextName, connectionString, options.UseDataAnnotations, options.SuppressConnectionStringWarning);

            var dbContextFileName = contextName + FileExtension;
            resultingFiles.ContextFile = new ScaffoldedFile { Path = Path.Combine(contextDir, dbContextFileName), Code = generatedCode };

            foreach (var entityType in model.GetEntityTypes())
            {
                generatedCode = CSharpEntityTypeGenerator.WriteCode(entityType, @namespace, options.UseDataAnnotations);

                var entityTypeFileName = entityType.DisplayName() + FileExtension;
                resultingFiles.AdditionalFiles.Add(new ScaffoldedFile { Path = entityTypeFileName, Code = generatedCode });
            }

            return resultingFiles;
        }

        /// <summary>
        /// Gets the programming language supported by this service.
        /// </summary>
        public override string Language => "C#";
    }
}