// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Generator for entity type classes using Handlebars templates.
    /// </summary>
    public class HbsTypeScriptEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        /// <summary>
        /// CSharp helper.
        /// </summary>
        protected ICSharpHelper CSharpHelper { get; }

        /// <summary>
        /// TypeScript helper.
        /// </summary>
        protected ITypeScriptHelper TypeScriptHelper { get; }

        private readonly IOptions<HandlebarsScaffoldingOptions> _options;

        /// <summary>
        /// Handlebars template data.
        /// </summary>
        protected Dictionary<string, object> TemplateData { get; private set; }

        /// <summary>
        /// Template service for the entity types generator.
        /// </summary>
        protected virtual IEntityTypeTemplateService EntityTypeTemplateService { get; }

        /// <summary>
        /// Service for transforming entity definitions.
        /// </summary>
        protected virtual IEntityTypeTransformationService EntityTypeTransformationService { get; }

        /// <summary>
        /// Constructor for the Handlebars entity types generator.
        /// </summary>
        /// <param name="annotationCodeGenerator">Annotation code generator.</param>
        /// <param name="entityTypeTemplateService">Template service for the entity types generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="cSharpHelper">CSharp helper.</param>
        /// <param name="typeScriptHelper">TypeScript helper.</param>
        /// <param name="options">Handlebars scaffolding options.</param>
        public HbsTypeScriptEntityTypeGenerator(
            [NotNull] IAnnotationCodeGenerator annotationCodeGenerator,
            [NotNull] IEntityTypeTemplateService entityTypeTemplateService,
            [NotNull] IEntityTypeTransformationService entityTypeTransformationService,
            [NotNull] ICSharpHelper cSharpHelper,
            [NotNull] ITypeScriptHelper typeScriptHelper,
            [NotNull] IOptions<HandlebarsScaffoldingOptions> options)
            : base(annotationCodeGenerator, cSharpHelper)
        {
            EntityTypeTemplateService = entityTypeTemplateService;
            EntityTypeTransformationService = entityTypeTransformationService;
            CSharpHelper = cSharpHelper;
            TypeScriptHelper = typeScriptHelper;
            _options =options;
        }

        /// <summary>
        /// Generate entity type class.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        /// <param name="namespace">Entity type namespace.</param>
        /// <param name="useDataAnnotations">If true use data annotations.</param>
        /// <returns>Generated entity type.</returns>
        public override string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(@namespace, nameof(@namespace));

            TemplateData = new Dictionary<string, object>();
            GenerateImports(entityType);
            GenerateClass(entityType);

            string output = EntityTypeTemplateService.GenerateEntityType(TemplateData);
            return output;
        }

        /// <summary>
        /// Generate entity type imports.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateImports(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var sortedNavigations = entityType.GetNavigations()
                .OrderBy(n => n.IsOnDependent ? 0 : 1)
                .ThenBy(n => n.IsCollection ? 1 : 0)
                .Distinct();

            if (sortedNavigations.Any())
            {
                var imports = new List<Dictionary<string, object>>();
                foreach (var navigation in sortedNavigations)
                {
                    imports.Add(new Dictionary<string, object> { { "import", navigation.TargetEntityType.Name } });
                }
                TemplateData.Add("imports", imports);
            }
        }

        /// <summary>
        /// Generate entity type class.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateClass(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var transformedEntityName = EntityTypeTransformationService.TransformTypeEntityName(entityType.Name);

            TemplateData.Add("comment", entityType.GetComment());
            TemplateData.Add("class", transformedEntityName);

            GenerateConstructor(entityType);
            GenerateProperties(entityType);
            GenerateNavigationProperties(entityType);
        }

        /// <summary>
        /// Generate entity type constructor.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateConstructor(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var collectionNavigations = entityType.GetNavigations().Where(n => n.IsCollection).ToList();

            if (collectionNavigations.Count > 0)
            {
                var lines = new List<Dictionary<string, object>>();

                foreach (var navigation in collectionNavigations)
                {
                    lines.Add(new Dictionary<string, object>
                    {
                        { "property-name", navigation.Name },
                        { "property-type", navigation.TargetEntityType.Name },
                    });
                }

                var transformedLines = EntityTypeTransformationService.TransformConstructor(entityType, lines);

                TemplateData.Add("lines", transformedLines);
            }
        }

        /// <summary>
        /// Generate entity type properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var properties = new List<Dictionary<string, object>>();

            foreach (var property in entityType.GetProperties().OrderBy(p => p.GetColumnOrdinal()))
            {
                properties.Add(new Dictionary<string, object>
                {
                    { "property-type", TypeScriptHelper.TypeName(property.ClrType) },
                    { "property-name",  TypeScriptHelper.ToCamelCase(property.Name) },
                    { "property-annotations",  new List<Dictionary<string, object>>() },
                    { "property-comment", property.GetComment() },
                    { "property-isnullable", property.IsNullable },
                    { "nullable-reference-types",  _options?.Value?.EnableNullableReferenceTypes == true }
                });
            }

            var transformedProperties = EntityTypeTransformationService.TransformProperties(entityType, properties);

            TemplateData.Add("properties", transformedProperties);
        }

        /// <summary>
        /// Generate entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateNavigationProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var sortedNavigations = entityType.GetNavigations()
                .OrderBy(n => n.IsOnDependent ? 0 : 1)
                .ThenBy(n => n.IsCollection ? 1 : 0);

            if (sortedNavigations.Any())
            {
                var navProperties = new List<Dictionary<string, object>>();

                foreach (var navigation in sortedNavigations)
                {
                    navProperties.Add(new Dictionary<string, object>
                    {
                        { "nav-property-collection", navigation.IsCollection },
                        { "nav-property-type", navigation.TargetEntityType.Name },
                        { "nav-property-name", TypeScriptHelper.ToCamelCase(navigation.Name) },
                        { "nav-property-annotations", new List<Dictionary<string, object>>() },
                        { "nullable-reference-types",  _options?.Value?.EnableNullableReferenceTypes == true }
                    });
                }

                var transformedNavProperties = EntityTypeTransformationService.TransformNavigationProperties(entityType, navProperties);

                TemplateData.Add("nav-properties", transformedNavProperties);
            }
        }
    }
}
