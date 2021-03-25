// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2020 Tony Sneed.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Generator for entity type classes using Handlebars templates.
    /// </summary>
    public class HbsEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        private readonly IOptions<HandlebarsScaffoldingOptions> _options;
        private readonly ILanguageOptions _languageOption;

        /// <summary>
        /// Annotation code generator.
        /// </summary>
        protected IAnnotationCodeGenerator AnnotationCodeGenerator { get; }

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
        /// Handlebars table annotations template data.
        /// </summary>
        protected List<Dictionary<string, object>> ClassAnnotationsData { get; set; }

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
        /// <param name="annotationCodeGenerator">Annotation code generator.</param>
        /// <param name="cSharpHelper">CSharp helper.</param>
        /// <param name="entityTypeTemplateService">Template service for the entity types generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="options">Handlebar scaffolding options.</param>
        /// <param name="languageOptions">Language Options.</param>
        public HbsEntityTypeGenerator(
            [NotNull] IAnnotationCodeGenerator annotationCodeGenerator,
            [NotNull] ICSharpHelper cSharpHelper,
            [NotNull] IEntityTypeTemplateService entityTypeTemplateService,
            [NotNull] IEntityTypeTransformationService entityTypeTransformationService,
            [NotNull] IOptions<HandlebarsScaffoldingOptions> options,
            [NotNull] ILanguageOptions languageOptions)
            : base(annotationCodeGenerator, cSharpHelper)
        {
            Check.NotNull(annotationCodeGenerator, nameof(annotationCodeGenerator));
            Check.NotNull(cSharpHelper, nameof(cSharpHelper));

            AnnotationCodeGenerator = annotationCodeGenerator;
            CSharpHelper = cSharpHelper;
            EntityTypeTemplateService = entityTypeTemplateService;
            EntityTypeTransformationService = entityTypeTransformationService;
            _options = options;
            _languageOption = languageOptions;
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

            // TODO: _sb.AppendLine("#nullable disable");
            @namespace = _options?.Value?.EnableSchemaFolders == true
                ? $"{@namespace}.{CSharpHelper.Namespace(entityType.GetSchema())}" : @namespace;

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

            var imports = _languageOption.EntityTypeImportListGenerator(entityType);

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

            var transformedEntityName = EntityTypeTransformationService.TransformTypeEntityName(entityType.Name);
            
            if (_options?.Value?.GenerateComments == true)
                TemplateData.Add("comment", GenerateComment(entityType.GetComment(), 1));
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
                .Where(n => n.IsCollection).ToList();

            if (collectionNavigations.Count > 0)
            {
                var lines = new List<Dictionary<string, object>>();

                foreach (var navigation in collectionNavigations)
                {
                    lines.Add(new Dictionary<string, object>
                    {
                        { "property-name", navigation.Name },
                        { "property-type", navigation.TargetEntityType.Name }
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
                
                var propertyType = CSharpHelper.Reference(property.ClrType);
                if (_options?.Value?.EnableNullableReferenceTypes == true 
                    && property.IsNullable
                    && !propertyType.EndsWith("?")) {
                        propertyType += "?";
                }
                properties.Add(new Dictionary<string, object>
                {
                    { "property-type", _languageOption.TypeNameConversion( propertyType ) },
                    { "property-name",  _languageOption.PropertyNameConversion( property.Name ) },
                    { "property-annotations",  PropertyAnnotationsData },
                    { "property-comment", _options?.Value?.GenerateComments == true ? GenerateComment(property.GetComment(), 2) : null },
                    { "property-isnullable", property.IsNullable },
                    { "nullable-reference-types", _options?.Value?.EnableNullableReferenceTypes == true }
                });
            }

            var transformedProperties = EntityTypeTransformationService.TransformProperties(properties);

            TemplateData.Add("properties", transformedProperties);
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

            var annotations = AnnotationCodeGenerator
                .FilterIgnoredAnnotations(property.GetAnnotations())
                .ToDictionary(a => a.Name, a => a);
            AnnotationCodeGenerator.RemoveAnnotationsHandledByConventions(property, annotations);

            foreach (var attribute in AnnotationCodeGenerator.GenerateDataAnnotationAttributes(property, annotations))
            {
                var attributeWriter = new AttributeWriter(attribute.Type.Name);
                foreach (var argument in attribute.Arguments)
                {
                    attributeWriter.AddParameter(CSharpHelper.UnknownLiteral(argument));
                }
            }
        }

        /// <summary>
        /// Generate entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected override void GenerateNavigationProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var sortedNavigations = entityType.GetScaffoldNavigations(_options.Value)
                .OrderBy(n => n.IsOnDependent ? 0 : 1)
                .ThenBy(n => n.IsCollection ? 1 : 0);

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

                    var propertyIsNullable = !navigation.IsCollection && (
                        navigation.IsOnDependent
                        ? !navigation.ForeignKey.IsRequired
                        : !navigation.ForeignKey.IsRequiredDependent
                    );
                    var navPropertyType = navigation.TargetEntityType.Name;
                    if (_options?.Value?.EnableNullableReferenceTypes == true &&
                        !navPropertyType.EndsWith("?") &&
                        propertyIsNullable) {
                        navPropertyType += "?";
                    }

                    navProperties.Add(new Dictionary<string, object>
                    {
                        { "nav-property-collection", navigation.IsCollection },
                        { "nav-property-type", navPropertyType },
                        { "nav-property-name", _languageOption.PropertyNameConversion( navigation.Name ) },
                        { "nav-property-annotations", NavPropertyAnnotations },
                        { "nav-property-isnullable", propertyIsNullable },
                        { "nullable-reference-types",  _options?.Value?.EnableNullableReferenceTypes == true }
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

            //GenerateTableAttribute(entityType);

            ClassAnnotationsData = new List<Dictionary<string, object>>();

            GenerateKeylessAttribute(entityType);
            GenerateTableAttribute(entityType);
            GenerateIndexAttributes(entityType);

            var annotations = AnnotationCodeGenerator
                .FilterIgnoredAnnotations(entityType.GetAnnotations())
                .ToDictionary(a => a.Name, a => a);
            AnnotationCodeGenerator.RemoveAnnotationsHandledByConventions(entityType, annotations);

            foreach (var attribute in AnnotationCodeGenerator.GenerateDataAnnotationAttributes(entityType, annotations))
            {
                var attributeWriter = new AttributeWriter(attribute.Type.Name);
                foreach (var argument in attribute.Arguments)
                {
                    attributeWriter.AddParameter(CSharpHelper.UnknownLiteral(argument));
                }
            }

            TemplateData.Add("class-annotations", ClassAnnotationsData);
        }

        private void GenerateKeylessAttribute(IEntityType entityType)
        {
            if (entityType.FindPrimaryKey() == null)
            {
                ClassAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "class-annotation", new AttributeWriter(nameof(KeylessAttribute)) }
                });
            }
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

                ClassAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "class-annotation", tableAttribute }
                });
            }
        }

        private void GenerateIndexAttributes(IEntityType entityType)
        {
            // Do not generate IndexAttributes for indexes which
            // would be generated anyway by convention.
            foreach (var index in entityType.GetIndexes().Where(
                i => ConfigurationSource.Convention != ((IConventionIndex)i).GetConfigurationSource()))
            {
                // If there are annotations that cannot be represented using an IndexAttribute then use fluent API instead.
                var annotations = AnnotationCodeGenerator
                    .FilterIgnoredAnnotations(index.GetAnnotations())
                    .ToDictionary(a => a.Name, a => a);
                AnnotationCodeGenerator.RemoveAnnotationsHandledByConventions(index, annotations);

                if (annotations.Count == 0)
                {
                    var indexAttribute = new AttributeWriter(nameof(IndexAttribute));
                    foreach (var property in index.Properties)
                    {
                        indexAttribute.AddParameter($"nameof({property.Name})");
                    }

                    if (index.Name != null)
                    {
                        indexAttribute.AddParameter($"{nameof(IndexAttribute.Name)} = {CSharpHelper.Literal(index.Name)}");
                    }

                    if (index.IsUnique)
                    {
                        indexAttribute.AddParameter($"{nameof(IndexAttribute.IsUnique)} = {CSharpHelper.Literal(index.IsUnique)}");
                    }

                    ClassAnnotationsData.Add(new Dictionary<string, object>
                    {
                        { "class-annotation", indexAttribute }
                    });
                }
            }
        }

        private void GenerateKeyAttribute(IProperty property)
        {
            var key = property.FindContainingPrimaryKey();
            if (key != null)
            {
                PropertyAnnotationsData.Add(new Dictionary<string, object>
                {
                    { "property-annotation", new AttributeWriter(nameof(KeyAttribute)) }
                });
            }
        }

        private void GenerateColumnAttribute(IProperty property)
        {
            var columnName = property.GetColumnBaseName();
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
                    { "property-annotation", columnAttribute }
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
                    { "property-annotation", lengthAttribute.ToString() }
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
                    { "property-annotation", new AttributeWriter(nameof(RequiredAttribute)) }
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
            if (navigation.IsOnDependent)
            {
                if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
                {
                    var foreignKeyAttribute = new AttributeWriter(nameof(ForeignKeyAttribute));

                    if (navigation.ForeignKey.Properties.Count > 1)
                    {
                        foreignKeyAttribute.AddParameter(
                                CSharpHelper.Literal(
                                    string.Join(",", navigation.ForeignKey.Properties.Select(p => EntityTypeTransformationService.TransformNavPropertyName(p.Name, p.ClrType.Name)))));
                    }
                    else
                    {
                        foreignKeyAttribute.AddParameter($"nameof({EntityTypeTransformationService.TransformNavPropertyName(navigation.ForeignKey.Properties.First().Name, navigation.ForeignKey.Properties.First().ClrType.Name)})");
                    }

                    NavPropertyAnnotations.Add(new Dictionary<string, object>
                    {
                        { "nav-property-annotation", foreignKeyAttribute }
                    });
                }
            }
        }

        private void GenerateInversePropertyAttribute(INavigation navigation)
        {
            if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
            {
                var inverseNavigation = navigation.Inverse;

                if (inverseNavigation != null)
                {
                    var inversePropertyAttribute = new AttributeWriter(nameof(InversePropertyAttribute));

                    var propertyName = EntityTypeTransformationService.TransformNavPropertyName(inverseNavigation.Name, navigation.DeclaringType.Name);
                    inversePropertyAttribute.AddParameter(
                        !navigation.DeclaringEntityType.GetPropertiesAndNavigations().Any(
                                m => m.Name == inverseNavigation.DeclaringEntityType.Name ||
                                    EntityTypeTransformationService.TransformNavPropertyName(m.Name, navigation.TargetEntityType.Name) 
                                        == EntityTypeTransformationService.TransformNavPropertyName(inverseNavigation.DeclaringEntityType.Name, navigation.TargetEntityType.Name))
                            ? $"nameof({EntityTypeTransformationService.TransformTypeEntityName(inverseNavigation.DeclaringType.Name)}.{propertyName})"
                            : CSharpHelper.Literal(propertyName));

                    NavPropertyAnnotations.Add(new Dictionary<string, object>
                    {
                        { "nav-property-annotation", inversePropertyAttribute }
                    });
                }
            }
        }

        private string GenerateComment(string comment, int indents)
        {
            if (!_languageOption.AddTripleSlashToComments)
                return comment;

            var sb = new IndentedStringBuilder();
            if (!string.IsNullOrWhiteSpace(comment))
            {
                for (int i = 0; i < indents; i++)
                    sb.IncrementIndent();
                foreach (var line in comment.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
                {
                    sb.AppendLine($"/// {System.Security.SecurityElement.Escape(line)}");
                }
                for (int i = 0; i < indents; i++)
                    sb.DecrementIndent();
            }
            return sb.ToString().Trim(Environment.NewLine.ToCharArray());
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
