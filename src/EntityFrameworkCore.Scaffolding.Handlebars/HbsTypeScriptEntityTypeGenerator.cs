// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Generator for entity type classes using Handlebars templates.
    /// </summary>
    public class HbsTypeScriptEntityTypeGenerator : ICSharpEntityTypeGenerator
    {
        /// <summary>
        /// CSharp helper.
        /// </summary>
        protected ICSharpHelper CSharpHelper { get; }

        /// <summary>
        /// TypeScript helper.
        /// </summary>
        protected ITypeScriptHelper TypeScriptHelper { get; }

        /// <summary>
        /// Handlebars template data.
        /// </summary>
        protected Dictionary<string, object> TemplateData { get; private set; }

        /// <summary>
        /// Template service for the entity types generator.
        /// </summary>
        public virtual IEntityTypeTemplateService EntityTypeTemplateService { get; }

        /// <summary>
        /// Service for transforming entity definitions.
        /// </summary>
        public virtual IEntityTypeTransformationService EntityTypeTransformationService { get; }

        /// <summary>
        /// Constructor for the Handlebars entity types generator.
        /// </summary>
        /// <param name="entityTypeTemplateService">Template service for the entity types generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="cSharpHelper">CSharp helper.</param>
        /// <param name="typeScriptHelper">TypeScript helper.</param>
        public HbsTypeScriptEntityTypeGenerator(
            IEntityTypeTemplateService entityTypeTemplateService,
            IEntityTypeTransformationService entityTypeTransformationService,
            ICSharpHelper cSharpHelper,
            ITypeScriptHelper typeScriptHelper)
        {
            CSharpHelper = cSharpHelper ?? throw new ArgumentNullException(nameof(cSharpHelper));
            TypeScriptHelper = typeScriptHelper ?? throw new ArgumentNullException(nameof(typeScriptHelper));
            EntityTypeTemplateService = entityTypeTemplateService ?? throw new ArgumentNullException(nameof(entityTypeTemplateService));
            EntityTypeTransformationService = entityTypeTransformationService ?? throw new ArgumentNullException(nameof(entityTypeTransformationService));
        }

        /// <summary>
        /// Generate entity type class.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        /// <param name="namespace">Entity type namespace.</param>
        /// <param name="useDataAnnotations">If true use data annotations.</param>
        /// <returns>Generated entity type.</returns>
        public virtual string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

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
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            var sortedNavigations = entityType.GetNavigations()
                .OrderBy(n => n.IsDependentToPrincipal() ? 0 : 1)
                .ThenBy(n => n.IsCollection() ? 1 : 0)
                .Distinct();

            if (sortedNavigations.Any())
            {
                var imports = new List<Dictionary<string, object>>();
                foreach (var navigation in sortedNavigations)
                {
                    var navigationName = navigation.GetTargetType().Name;
                    // Avoid duplicates in the list of imports
                    var newImport = new Dictionary<string, object> { { "import", navigationName } };
                    if (!imports.Any(d => d.ContainsValue(navigationName)))
                    {
                        imports.Add(newImport);
                    }
                }
                TemplateData.Add("imports", imports);
            }
        }

        /// <summary>
        /// Generate entity type class.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateClass(IEntityType entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            var transformedEntityName = EntityTypeTransformationService.TransformEntityName(entityType.Name);

            TemplateData.Add("class", transformedEntityName);

            GenerateConstructor(entityType);
            GenerateProperties(entityType);
            GenerateNavigationProperties(entityType);
        }

        /// <summary>
        /// Generate entity type constructor.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateConstructor(IEntityType entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            var collectionNavigations = entityType.GetNavigations().Where(n => n.IsCollection()).ToList();

            if (collectionNavigations.Count > 0)
            {
                var lines = new List<Dictionary<string, object>>();

                foreach (var navigation in collectionNavigations)
                {
                    lines.Add(new Dictionary<string, object>
                    {
                        { "property-name", navigation.Name },
                        { "property-type", navigation.GetTargetType().Name },
                    });
                }

                var transformedLines = EntityTypeTransformationService.TransformConstructor(lines);

                TemplateData.Add("lines", transformedLines);
            }
        }

        /// <summary>
        /// Generate entity type properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateProperties(IEntityType entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            var properties = new List<Dictionary<string, object>>();

            foreach (var property in entityType.GetProperties().OrderBy(p => p.Scaffolding().ColumnOrdinal))
            {
                properties.Add(new Dictionary<string, object>
                {
                    { "property-type", TypeScriptHelper.TypeName(property.ClrType) },
                    { "property-name",  TypeScriptHelper.ToCamelCase(property.Name) },
                    { "property-annotations",  new List<Dictionary<string, object>>() },
                });
            }

            var transformedProperties = EntityTypeTransformationService.TransformProperties(properties);

            TemplateData.Add("properties", transformedProperties);
        }

        /// <summary>
        /// Generate entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateNavigationProperties(IEntityType entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            var sortedNavigations = entityType.GetNavigations()
                .OrderBy(n => n.IsDependentToPrincipal() ? 0 : 1)
                .ThenBy(n => n.IsCollection() ? 1 : 0);

            if (sortedNavigations.Any())
            {
                var navProperties = new List<Dictionary<string, object>>();

                foreach (var navigation in sortedNavigations)
                {
                    navProperties.Add(new Dictionary<string, object>
                    {
                        { "nav-property-collection", navigation.IsCollection() },
                        { "nav-property-type", navigation.GetTargetType().Name },
                        { "nav-property-name", TypeScriptHelper.ToCamelCase(navigation.Name) },
                        { "nav-property-annotations", new List<Dictionary<string, object>>() },
                    });
                }

                var transformedNavProperties = EntityTypeTransformationService.TransformNavigationProperties(navProperties);

                TemplateData.Add("nav-properties", transformedNavProperties);
            }
        }
    }
}
