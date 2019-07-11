// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Scaffolding generator for TypeScript entity type classes using Handlebars templates.
    /// </summary>
    public class HbsTypeScriptModelGenerator : ModelCodeGenerator
    {
        private const string FileExtension = ".ts";

        /// <summary>
        /// Handlebars helper service.
        /// </summary>
        public virtual IHbsHelperService HandlebarsHelperService { get; }

        /// <summary>
        /// Handlebars helper service.
        /// </summary>
        public virtual IHbsBlockHelperService HandlebarsBlockHelperService { get; }

        /// <summary>
        /// DbContext template service.
        /// </summary>
        public virtual IDbContextTemplateService DbContextTemplateService { get; }

        /// <summary>
        /// Entity type template service.
        /// </summary>
        public virtual IEntityTypeTemplateService EntityTypeTemplateService { get; }

        /// <summary>
        /// Service for transforming entity definitions.
        /// </summary>
        public virtual IEntityTypeTransformationService EntityTypeTransformationService { get; }

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
        /// <param name="handlebarsHelperService">Handlebars helper service.</param>
        /// <param name="handlebarsBlockHelperService">Handlebars block helper service.</param>
        /// <param name="dbContextTemplateService">Template service for DbContext generator.</param>
        /// <param name="entityTypeTemplateService">Template service for the entity types generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="cSharpDbContextGenerator">DbContext generator.</param>
        /// <param name="cSharpEntityTypeGenerator">Entity type generator.</param>
        public HbsTypeScriptModelGenerator(ModelCodeGeneratorDependencies dependencies,
            IHbsHelperService handlebarsHelperService,
            IHbsBlockHelperService handlebarsBlockHelperService,
            IDbContextTemplateService dbContextTemplateService,
            IEntityTypeTemplateService entityTypeTemplateService,
            IEntityTypeTransformationService entityTypeTransformationService,
            ICSharpDbContextGenerator cSharpDbContextGenerator,
            ICSharpEntityTypeGenerator cSharpEntityTypeGenerator) : base(dependencies)
        {
            HandlebarsHelperService = handlebarsHelperService ?? throw new ArgumentNullException(nameof(handlebarsHelperService));
            HandlebarsBlockHelperService = handlebarsBlockHelperService ?? throw new ArgumentNullException(nameof(handlebarsBlockHelperService));
            DbContextTemplateService = dbContextTemplateService ?? throw new ArgumentNullException(nameof(dbContextTemplateService));
            EntityTypeTemplateService = entityTypeTemplateService ?? throw new ArgumentNullException(nameof(entityTypeTemplateService));
            EntityTypeTransformationService = entityTypeTransformationService ?? throw new ArgumentNullException(nameof(entityTypeTransformationService));
            CSharpDbContextGenerator = cSharpDbContextGenerator ?? throw new ArgumentNullException(nameof(cSharpDbContextGenerator));
            CSharpEntityTypeGenerator = cSharpEntityTypeGenerator ?? throw new ArgumentNullException(nameof(cSharpEntityTypeGenerator));
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
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentNullException(nameof(@namespace));
            if (contextDir == null) throw new ArgumentNullException(nameof(contextDir));
            if (string.IsNullOrWhiteSpace(contextName)) throw new ArgumentNullException(nameof(contextName));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (contextDir == null) throw new ArgumentNullException(nameof(contextDir));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Register Hbs helpers and partial templates
            HandlebarsHelperService.RegisterHelpers();
            HandlebarsBlockHelperService.RegisterBlockHelpers();
            DbContextTemplateService.RegisterPartialTemplates();
            EntityTypeTemplateService.RegisterPartialTemplates();

            var resultingFiles = new ScaffoldedModel();

            string generatedCode;

            if (!(CSharpDbContextGenerator is NullCSharpDbContextGenerator))
            {
                generatedCode = CSharpDbContextGenerator.WriteCode(model, @namespace, contextName, connectionString, options.UseDataAnnotations, options.SuppressConnectionStringWarning);

                var dbContextFileName = contextName + ".cs";
                resultingFiles.ContextFile = new ScaffoldedFile { Path = Path.Combine(contextDir, dbContextFileName), Code = generatedCode };
            }

            if (!(CSharpEntityTypeGenerator is NullCSharpEntityTypeGenerator))
            {
                foreach (var entityType in model.GetEntityTypes())
                {
                    generatedCode = CSharpEntityTypeGenerator.WriteCode(entityType, @namespace, options.UseDataAnnotations);

                    var transformedFileName = EntityTypeTransformationService.TransformEntityFileName(entityType.DisplayName());
                    var entityTypeFileName = transformedFileName + FileExtension;
                    resultingFiles.AdditionalFiles.Add(new ScaffoldedFile { Path = entityTypeFileName, Code = generatedCode });
                }
            }

            return resultingFiles;
        }

        /// <summary>
        /// Gets the programming language supported by this service.
        /// </summary>
        public override string Language => "C#";
    }
}