// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Scaffolding generator for DbContext and entity type classes using Handlebars templates.
    /// </summary>
    public class HbsCSharpModelGenerator : CSharpModelGenerator
    {
        private const string FileExtension = ".cs";
        private readonly IOptions<HandlebarsScaffoldingOptions> _options;

        /// <summary>
        /// Handlebars helper service.
        /// </summary>
        protected virtual IHbsHelperService HandlebarsHelperService { get; }

        /// <summary>
        /// Handlebars helper service.
        /// </summary>
        protected virtual IHbsBlockHelperService HandlebarsBlockHelperService { get; }

        /// <summary>
        /// DbContext template service.
        /// </summary>
        protected virtual IDbContextTemplateService DbContextTemplateService { get; }

        /// <summary>
        /// Entity type template service.
        /// </summary>
        protected virtual IEntityTypeTemplateService EntityTypeTemplateService { get; }

        /// <summary>
        /// Service for transforming entity definitions.
        /// </summary>
        protected virtual IEntityTypeTransformationService EntityTypeTransformationService { get; }

        /// <summary>
        /// Service for transforming context definitions.
        /// </summary>
        protected IContextTransformationService ContextTransformationService { get; }

        /// <summary>
        /// CSharp helper.
        /// </summary>
        public ICSharpHelper CSharpHelper { get; }

        /// <summary>
        /// Constructor for the HbsCSharpModelGenerator.
        /// </summary>
        /// <param name="dependencies">Service dependencies parameter class for HbsCSharpModelGenerator.</param>
        /// <param name="cSharpDbContextGenerator">DbContext generator.</param>
        /// <param name="cSharpEntityTypeGenerator">Entity type generator.</param>
        /// <param name="handlebarsHelperService">Handlebars helper service.</param>
        /// <param name="handlebarsBlockHelperService">Handlebars block helper service.</param>
        /// <param name="dbContextTemplateService">Template service for DbContext generator.</param>
        /// <param name="entityTypeTemplateService">Template service for the entity types generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="contextTransformationService">Service for transforming context definitions.</param>
        /// <param name="cSharpHelper">CSharp helper.</param>
        /// <param name="options">Handlebar scaffolding options.</param>
        public HbsCSharpModelGenerator(
            [NotNull] ModelCodeGeneratorDependencies dependencies,
            [NotNull] ICSharpDbContextGenerator cSharpDbContextGenerator,
            [NotNull] ICSharpEntityTypeGenerator cSharpEntityTypeGenerator,
            [NotNull] IHbsHelperService handlebarsHelperService,
            [NotNull] IHbsBlockHelperService handlebarsBlockHelperService,
            [NotNull] IDbContextTemplateService dbContextTemplateService,
            [NotNull] IEntityTypeTemplateService entityTypeTemplateService,
            [NotNull] IEntityTypeTransformationService entityTypeTransformationService,
            [NotNull] IContextTransformationService contextTransformationService,
            [NotNull] ICSharpHelper cSharpHelper,
            [NotNull] IOptions<HandlebarsScaffoldingOptions> options)
            : base(dependencies, cSharpDbContextGenerator, cSharpEntityTypeGenerator)
        {
            HandlebarsHelperService = handlebarsHelperService;
            HandlebarsBlockHelperService = handlebarsBlockHelperService;
            DbContextTemplateService = dbContextTemplateService;
            EntityTypeTemplateService = entityTypeTemplateService;
            EntityTypeTransformationService = entityTypeTransformationService;
            ContextTransformationService = contextTransformationService;
            CSharpHelper = cSharpHelper;
            _options = options;
        }

        /// <summary>
        /// <summary>Generates code for a model.</summary>
        /// </summary>
        /// <param name="model"> The model.</param>
        /// <param name="options"> The options to use during generation. </param>
        /// <returns> The generated model. </returns>
        public override ScaffoldedModel GenerateModel(IModel model, ModelCodeGenerationOptions options)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(options, nameof(options));

            // Register Hbs helpers and partial templates
            HandlebarsHelperService.RegisterHelpers();
            HandlebarsBlockHelperService.RegisterBlockHelpers();
            DbContextTemplateService.RegisterPartialTemplates();
            EntityTypeTemplateService.RegisterPartialTemplates();

            var resultingFiles = new ScaffoldedModel();

            string generatedCode;

            if (!(CSharpDbContextGenerator is NullCSharpDbContextGenerator))
            {
                generatedCode = CSharpDbContextGenerator.WriteCode(
                    model,
                    options.ContextName,
                    options.ConnectionString,
                    options.ContextNamespace,
                    options.ModelNamespace,
                    options.UseDataAnnotations,
                    options.UseNullableReferenceTypes,
                    options.SuppressConnectionStringWarning,
                    options.SuppressOnConfiguring);

                var dbContextFileName = ContextTransformationService.TransformContextFileName(options.ContextName) + FileExtension;
                resultingFiles.ContextFile = new ScaffoldedFile
                {
                    Path = options.ContextDir != null
                        ? Path.Combine(options.ContextDir, dbContextFileName)
                        : dbContextFileName,
                    Code = generatedCode
                };
            }

            if (!(CSharpEntityTypeGenerator is NullCSharpEntityTypeGenerator))
            {
                foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
                {
                    if (IsManyToManyJoinEntityType(entityType))
                    {
                        continue;
                    }
                    generatedCode = CSharpEntityTypeGenerator.WriteCode(
                        entityType,
                        options.ModelNamespace,
                        options.UseDataAnnotations,
                        options.UseNullableReferenceTypes);

                    var transformedFileName = EntityTypeTransformationService.TransformEntityFileName(entityType.Name);
                    var schema = !string.IsNullOrEmpty(entityType.GetTableName())
                        ? entityType.GetSchema()
                        : entityType.GetViewSchema();
                    var entityTypeFileName = _options?.Value?.EnableSchemaFolders == true
                        ? Path.Combine(CSharpHelper.Namespace(schema), transformedFileName + FileExtension)
                        : transformedFileName + FileExtension;
                    resultingFiles.AdditionalFiles.Add(
                        new ScaffoldedFile
                        {
                            Path = entityTypeFileName,
                            Code = generatedCode
                        });
                }
            }

            return resultingFiles;
        }


        internal static bool IsManyToManyJoinEntityType(IEntityType entityType)
        {
            if (!entityType.GetNavigations().Any()
                && !entityType.GetSkipNavigations().Any())
            {
                var primaryKey = entityType.FindPrimaryKey();
                var properties = entityType.GetProperties().ToList();
                var foreignKeys = entityType.GetForeignKeys().ToList();
                if (primaryKey != null
                    && primaryKey.Properties.Count > 1
                    && foreignKeys.Count == 2
                    && primaryKey.Properties.Count == properties.Count
                    && foreignKeys[0].Properties.Count + foreignKeys[1].Properties.Count == properties.Count
                    && !foreignKeys[0].Properties.Intersect(foreignKeys[1].Properties).Any()
                    && foreignKeys[0].IsRequired
                    && foreignKeys[1].IsRequired
                    && !foreignKeys[0].IsUnique
                    && !foreignKeys[1].IsUnique)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
