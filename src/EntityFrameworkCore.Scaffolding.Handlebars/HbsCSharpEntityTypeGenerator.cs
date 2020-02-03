﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Generator for entity type classes using Handlebars templates.
    /// </summary>
    public class HbsCSharpEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        private readonly IOptions<HandlebarsScaffoldingOptions> _options;

        /// <summary>
        /// CSharp helper.
        /// </summary>
        protected ICSharpHelper CSharpHelper { get; }

        /// <summary>
        /// Use data annotations.
        /// </summary>
        protected bool UseDataAnnotations { get; set; }

        /// <summary>
        /// Handlebars template data.
        /// </summary>
        protected Dictionary<string, object> TemplateData { get; private set; }

        /// <summary>
        /// Handlebars property annotations template data.
        /// </summary>
        protected List<Dictionary<string, object>> PropertyAnnotationsData { get; set; }

        /// <summary>
        /// Handlebars navigation property annotations template data.
        /// </summary>
        protected List<Dictionary<string, object>> NavPropertyAnnotations { get; set; }

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
        /// <param name="cSharpHelper">CSharp helper.</param>
        /// <param name="entityTypeTemplateService">Template service for the entity types generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="options">Handlebar scaffolding options.</param>
        public HbsCSharpEntityTypeGenerator(
            [NotNull] ICSharpHelper cSharpHelper,
            [NotNull] IEntityTypeTemplateService entityTypeTemplateService,
            [NotNull] IEntityTypeTransformationService entityTypeTransformationService,
            [NotNull] IOptions<HandlebarsScaffoldingOptions> options)
            : base(cSharpHelper)
        {
            CSharpHelper = cSharpHelper;
            EntityTypeTemplateService = entityTypeTemplateService;
            EntityTypeTransformationService = entityTypeTransformationService;
            _options = options;
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

            UseDataAnnotations = useDataAnnotations;
            TemplateData = new Dictionary<string, object>();

            if (_options.Value.TemplateData != null)
            {
                foreach (KeyValuePair<string, object> entry in _options.Value.TemplateData)
                {
                    TemplateData.Add(entry.Key, entry.Value);
                }
            }

            TemplateData.Add("use-data-annotations", UseDataAnnotations);

            GenerateImports(entityType);

            TemplateData.Add("namespace", @namespace);

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

            var imports = new List<Dictionary<string, object>>();

            foreach (var ns in entityType.GetProperties()
                .SelectMany(p => p.ClrType.GetNamespaces())
                .Where(ns => ns != "System" && ns != "System.Collections.Generic")
                .Distinct())
            {
                imports.Add(new Dictionary<string, object> { { "import", ns } });
            }

            TemplateData.Add("imports", imports);
        }

        /// <summary>
        /// Generate entity type class.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateClass(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            if (UseDataAnnotations)
            {
                GenerateEntityTypeDataAnnotations(entityType);
            }

            var transformedEntityName = EntityTypeTransformationService.TransformEntityName(entityType);

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

            var collectionNavigations = entityType.GetScaffoldNavigations(_options.Value)
                .Where(n => n.IsCollection()).ToList();

            if (collectionNavigations.Count > 0)
            {
                var lines = new List<Dictionary<string, object>>();

                foreach (var navigation in collectionNavigations)
                {
                    lines.Add(new Dictionary<string, object>
                    {
                        { "property-name", navigation.Name },
                        { "property-type", navigation.GetTargetType().Name },
                        { "property-isnullable", null }
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
        protected override void GenerateProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var properties = new List<Dictionary<string, object>>();

            foreach (var property in entityType.GetProperties().OrderBy(p => p.GetColumnOrdinal()))
            {
                PropertyAnnotationsData = new List<Dictionary<string, object>>();

                if (UseDataAnnotations)
                {
                    GeneratePropertyDataAnnotations(property);
                }

                properties.Add(new Dictionary<string, object>
                {
                    { "property-type", CSharpHelper.Reference(property.ClrType) },
                    { "property-name", property.Name },
                    { "property-annotations",  PropertyAnnotationsData },
                    { "property-isnullable",  property.IsNullable }
                });
            }

            var transformedProperties = EntityTypeTransformationService.TransformProperties(properties);

            TemplateData.Add("properties", transformedProperties);
        }

        /// <summary>
        /// Generate entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateNavigationProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var sortedNavigations = entityType.GetScaffoldNavigations(_options.Value)
                .OrderBy(n => n.IsDependentToPrincipal() ? 0 : 1)
                .ThenBy(n => n.IsCollection() ? 1 : 0);

            if (sortedNavigations.Any())
            {
                var navProperties = new List<Dictionary<string, object>>();

                foreach (var navigation in sortedNavigations)
                {
                    NavPropertyAnnotations = new List<Dictionary<string, object>>();

                    if (UseDataAnnotations)
                    {
                        GenerateNavigationDataAnnotations(navigation);
                    }

                    navProperties.Add(new Dictionary<string, object>
                    {
                        { "nav-property-collection", navigation.IsCollection() },
                        { "nav-property-type", navigation.GetTargetType().Name },
                        { "nav-property-name", navigation.Name },
                        { "nav-property-annotations", NavPropertyAnnotations },
                    });
                }

                var transformedNavProperties = EntityTypeTransformationService.TransformNavigationProperties(navProperties);

                TemplateData.Add("nav-properties", transformedNavProperties);
            }
        }

        /// <summary>
        /// Generate entity type data annotations.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateEntityTypeDataAnnotations(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            GenerateTableAttribute(entityType);
        }

        /// <summary>
        /// Generate property data annotations.
        /// </summary>
        /// <param name="property">Represents a scalar property of an entity.</param>
        protected override void GeneratePropertyDataAnnotations(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            GenerateKeyAttribute(property);
            GenerateRequiredAttribute(property);
            GenerateColumnAttribute(property);
            GenerateMaxLengthAttribute(property);
        }

        private void GenerateTableAttribute(IEntityType entityType)
        {
            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema();
            var defaultSchema = entityType.Model.GetDefaultSchema();

            var schemaParameterNeeded = schema != null && schema != defaultSchema;
            var tableAttributeNeeded = schemaParameterNeeded || tableName != null && tableName != entityType.GetDbSetName();

            if (tableAttributeNeeded)
            {
                var tableAttribute = new AttributeWriter(nameof(TableAttribute));

                tableAttribute.AddParameter(CSharpHelper.Literal(tableName));

                if (schemaParameterNeeded)
                {
                    tableAttribute.AddParameter($"{nameof(TableAttribute.Schema)} = {CSharpHelper.Literal(schema)}");
                }

                TemplateData.Add("class-annotation", tableAttribute.ToString());
            }
        }

        private void GenerateKeyAttribute(IProperty property)
        {
            var key = property.FindContainingPrimaryKey();
            if (key != null)
            {
                PropertyAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "property-annotation", new AttributeWriter(nameof(KeyAttribute)) },
                });
            }
        }

        private void GenerateColumnAttribute(IProperty property)
        {
            var columnName = property.GetColumnName();
            var columnType = property.GetConfiguredColumnType();

            var delimitedColumnName = columnName != null && columnName != property.Name ? CSharpHelper.Literal(columnName) : null;
            var delimitedColumnType = columnType != null ? CSharpHelper.Literal(columnType) : null;

            if ((delimitedColumnName ?? delimitedColumnType) != null)
            {
                var columnAttribute = new AttributeWriter(nameof(ColumnAttribute));

                if (delimitedColumnName != null)
                {
                    columnAttribute.AddParameter(delimitedColumnName);
                }

                if (delimitedColumnType != null)
                {
                    columnAttribute.AddParameter($"{nameof(ColumnAttribute.TypeName)} = {delimitedColumnType}");
                }

                PropertyAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "property-annotation", columnAttribute },
                });
            }
        }

        private void GenerateMaxLengthAttribute(IProperty property)
        {
            var maxLength = property.GetMaxLength();

            if (maxLength.HasValue)
            {
                var lengthAttribute = new AttributeWriter(
                    property.ClrType == typeof(string)
                        ? nameof(StringLengthAttribute)
                        : nameof(MaxLengthAttribute));

                lengthAttribute.AddParameter(CSharpHelper.Literal(maxLength.Value));

                PropertyAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "property-annotation", lengthAttribute.ToString() },
                });
            }
        }

        private void GenerateRequiredAttribute(IProperty property)
        {
            if (!property.IsNullable
                && property.ClrType.IsNullableType()
                && !property.IsPrimaryKey())
            {
                PropertyAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "property-annotation", new AttributeWriter(nameof(RequiredAttribute)).ToString() },
                });
            }
        }

        private void GenerateNavigationDataAnnotations(INavigation navigation)
        {
            if (navigation == null) throw new ArgumentNullException(nameof(navigation));

            GenerateForeignKeyAttribute(navigation);
            GenerateInversePropertyAttribute(navigation);
        }

        private void GenerateForeignKeyAttribute(INavigation navigation)
        {
            if (navigation.IsDependentToPrincipal())
            {
                if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
                {
                    var foreignKeyAttribute = new AttributeWriter(nameof(ForeignKeyAttribute));

                    foreignKeyAttribute.AddParameter(
                        CSharpHelper.Literal(
                            string.Join(",", navigation.ForeignKey.Properties.Select(p => p.Name))));

                    NavPropertyAnnotations.Add(new Dictionary<string, object>
                    {
                        { "nav-property-annotation", foreignKeyAttribute.ToString() },
                    });
                }
            }
        }

        private void GenerateInversePropertyAttribute(INavigation navigation)
        {
            if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
            {
                var inverseNavigation = navigation.FindInverse();

                if (inverseNavigation != null)
                {
                    var inversePropertyAttribute = new AttributeWriter(nameof(InversePropertyAttribute));

                    inversePropertyAttribute.AddParameter(CSharpHelper.Literal(inverseNavigation.Name));

                    NavPropertyAnnotations.Add(new Dictionary<string, object>
                    {
                        { "nav-property-annotation", inversePropertyAttribute.ToString() },
                    });
                }
            }
        }

        private class AttributeWriter
        {
            private readonly string _attibuteName;
            private readonly List<string> _parameters = new List<string>();

            public AttributeWriter(string attributeName)
            {
                _attibuteName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            }

            public void AddParameter(string parameter)
            {
                if (parameter == null) throw new ArgumentNullException(nameof(parameter));

                _parameters.Add(parameter);
            }

            public override string ToString()
                => "[" + (_parameters.Count == 0
                       ? StripAttribute(_attibuteName)
                       : StripAttribute(_attibuteName) + "(" + string.Join(", ", _parameters) + ")") + "]";

            private static string StripAttribute(string attributeName)
            {
                if (attributeName == null) throw new ArgumentNullException(nameof(attributeName));
                return attributeName.EndsWith("Attribute", StringComparison.Ordinal)
                    ? attributeName.Substring(0, attributeName.Length - 9)
                    : attributeName;
            }
        }
    }
}
