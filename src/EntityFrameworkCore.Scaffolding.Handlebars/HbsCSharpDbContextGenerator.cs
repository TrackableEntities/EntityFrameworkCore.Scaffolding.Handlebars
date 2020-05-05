// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Generator for the DbContext class using Handlebars templates.
    /// </summary>
    public class HbsCSharpDbContextGenerator : CSharpDbContextGenerator
    {
        private const string EntityLambdaIdentifier = "entity";
        private const string Language = "CSharp";
        private readonly IOptions<HandlebarsScaffoldingOptions> _options;
        private string _modelNamespace;

        /// <summary>
        /// CSharp helper.
        /// </summary>
        protected ICSharpHelper CSharpHelper { get; }

        /// <summary>
        /// Annotation code generator.
        /// </summary>
        protected IAnnotationCodeGenerator AnnotationCodeGenerator { get; }

        /// <summary>
        /// Generator for scaffolding provider.
        /// </summary>
        protected IProviderConfigurationCodeGenerator ProviderConfigurationCodeGenerator { get; }

        /// <summary>
        /// DbContext template service.
        /// </summary>
        protected IDbContextTemplateService DbContextTemplateService { get; }

        /// <summary>
        /// Service for transforming entity definitions.
        /// </summary>
        protected IEntityTypeTransformationService EntityTypeTransformationService { get; }

        /// <summary>
        /// Handlebars template data.
        /// </summary>
        protected Dictionary<string, object> TemplateData { get; set; }

        private bool _entityTypeBuilderInitialized;

        /// <summary>
        /// Constructor for the Handlebars DbContext generator.
        /// </summary>
        /// <param name="providerConfigurationCodeGenerator">Generator for scaffolding provider.</param>
        /// <param name="annotationCodeGenerator">Annotation code generator.</param>
        /// <param name="cSharpHelper">CSharp helper.</param>
        /// <param name="dbContextTemplateService">Template service for DbContext generator.</param>
        /// <param name="entityTypeTransformationService">Service for transforming entity definitions.</param>
        /// <param name="options">Handlebars scaffolding options.</param>
        public HbsCSharpDbContextGenerator(
            [NotNull] IProviderConfigurationCodeGenerator providerConfigurationCodeGenerator, 
            [NotNull] IAnnotationCodeGenerator annotationCodeGenerator,
            [NotNull] IDbContextTemplateService dbContextTemplateService,
            [NotNull] IEntityTypeTransformationService entityTypeTransformationService,
            [NotNull] ICSharpHelper cSharpHelper,
            [NotNull] IOptions<HandlebarsScaffoldingOptions> options)
            : base(providerConfigurationCodeGenerator, annotationCodeGenerator, cSharpHelper)
        {
            ProviderConfigurationCodeGenerator = providerConfigurationCodeGenerator;
            AnnotationCodeGenerator = annotationCodeGenerator;
            CSharpHelper = cSharpHelper;
            DbContextTemplateService = dbContextTemplateService;
            EntityTypeTransformationService = entityTypeTransformationService;
            _options = options;
        }

        /// <summary>
        /// Generate the DbContext class.
        /// </summary>
        /// <param name="model">Metadata about the shape of entities, the relationships between them, and how they map to the database.</param>
        /// <param name="contextName">Name of DbContext class.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="contextNamespace">Context namespace.</param>
        /// <param name="modelNamespace">Model namespace.</param>
        /// <param name="useDataAnnotations">If false use fluent modeling API.</param>
        /// <param name="suppressConnectionStringWarning">Suppress connection string warning.</param>
        /// <returns>DbContext class.</returns>
        public override string WriteCode(IModel model, string contextName, string connectionString,
            string contextNamespace, string modelNamespace, bool useDataAnnotations, bool suppressConnectionStringWarning)
        {
            Check.NotNull(model, nameof(model));

            if (!string.IsNullOrEmpty(modelNamespace) && string.CompareOrdinal(contextNamespace, modelNamespace) != 0)
                _modelNamespace = modelNamespace;

            TemplateData = new Dictionary<string, object>();

            if (_options.Value.TemplateData != null)
            {
                foreach (KeyValuePair<string, object> entry in _options.Value.TemplateData)
                {
                    TemplateData.Add(entry.Key, entry.Value);
                }
            }

            TemplateData.Add("namespace", contextNamespace);

            GenerateClass(model, contextName, connectionString, useDataAnnotations, suppressConnectionStringWarning);

            string output = DbContextTemplateService.GenerateDbContext(TemplateData);
            return output;
        }

        /// <summary>
        /// Generate the DbContext class.
        /// </summary>
        /// <param name="model">Metadata about the shape of entities, the relationships between them, and how they map to the database.</param>
        /// <param name="contextName">Name of DbContext class.</param>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="useDataAnnotations">Use fluent modeling API if false.</param>
        /// <param name="suppressConnectionStringWarning">Suppress connection string warning.</param>
        protected override void GenerateClass(
            IModel model, string contextName, string connectionString, bool useDataAnnotations, 
            bool suppressConnectionStringWarning)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(contextName, nameof(contextName));
            Check.NotNull(connectionString, nameof(connectionString));

            TemplateData.Add("model-namespace", _modelNamespace);
            TemplateData.Add("class", contextName);

            GenerateDbSets(model);
            GenerateEntityTypeErrors(model);
            GenerateOnConfiguring(connectionString, suppressConnectionStringWarning);
            GenerateOnModelCreating(model, useDataAnnotations);
        }

        /// <summary>
        /// Generate OnConfiguring method.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="suppressConnectionStringWarning">Suppress connection string warning.</param>
        protected override void GenerateOnConfiguring(string connectionString, bool suppressConnectionStringWarning)
        {
            Check.NotNull(connectionString, nameof(connectionString));

            var sb = new IndentedStringBuilder();
            using (sb.Indent())
            using (sb.Indent())
            {
                sb.AppendLine("protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
                sb.AppendLine("{");

                using (sb.Indent())
                {
                    sb.AppendLine("if (!optionsBuilder.IsConfigured)");
                    sb.AppendLine("{");

                    using (sb.Indent())
                    {
                        if (!suppressConnectionStringWarning)
                        {
                            sb.DecrementIndent()
                                .DecrementIndent()
                                .DecrementIndent()
                                .DecrementIndent()
                                .AppendLine("#warning " + DesignStrings.SensitiveInformationWarning)
                                .IncrementIndent()
                                .IncrementIndent()
                                .IncrementIndent()
                                .IncrementIndent();
                        }

                        sb.Append("optionsBuilder");

                        var useProviderCall = ProviderConfigurationCodeGenerator.GenerateUseProvider(
                            connectionString,
                            ProviderConfigurationCodeGenerator.GenerateProviderOptions());
                        var contextOptions = ProviderConfigurationCodeGenerator.GenerateContextOptions();
                        if (contextOptions != null)
                        {
                            useProviderCall = useProviderCall.Chain(contextOptions);
                        }

                        sb.Append(CSharpHelper.Fragment(useProviderCall))
                          .AppendLine(";");
                    }
                    sb.AppendLine("}");
                }

                sb.AppendLine("}"); 
            }

            var onConfiguring = sb.ToString();
            TemplateData.Add("on-configuring", onConfiguring);
        }

        /// <summary>
        /// Generate OnModelBuilding method.
        /// </summary>
        /// <param name="model">Metadata about the shape of entities, the relationships between them, and how they map to the database.</param>
        /// <param name="useDataAnnotations">Use fluent modeling API if false.</param>
        protected override void GenerateOnModelCreating(IModel model, bool useDataAnnotations)
        {
            Check.NotNull(model, nameof(model));

            var sb = new IndentedStringBuilder();
            using (sb.Indent())
            using (sb.Indent())
            {
                sb.AppendLine("protected override void OnModelCreating(ModelBuilder modelBuilder)");
                sb.Append("{");

                var annotations = model.GetAnnotations().ToList();
                RemoveAnnotation(ref annotations, CoreAnnotationNames.ProductVersion);
                RemoveAnnotation(ref annotations, CoreAnnotationNames.ChangeTrackingStrategy);
                RemoveAnnotation(ref annotations, CoreAnnotationNames.OwnedTypes);
                RemoveAnnotation(ref annotations, ChangeDetector.SkipDetectChangesAnnotation);
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.MaxIdentifierLength);
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.CheckConstraints);
                RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.DatabaseName);
                RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.EntityTypeErrors);

                var annotationsToRemove = new List<IAnnotation>();
                annotationsToRemove.AddRange(
                    annotations.Where(
                        a => a.Name.StartsWith(RelationalAnnotationNames.SequencePrefix, StringComparison.Ordinal)));

                var lines = new List<string>();

                foreach (var annotation in annotations)
                {
                    if (annotation.Value == null
                        || AnnotationCodeGenerator.IsHandledByConvention(model, annotation))
                    {
                        annotationsToRemove.Add(annotation);
                    }
                    else
                    {
                        var methodCall = AnnotationCodeGenerator.GenerateFluentApi(model, annotation);
                        if (methodCall != null)
                        {
                            lines.Add(CSharpHelper.Fragment(methodCall));
                            annotationsToRemove.Add(annotation);
                        }
                    }
                }

                lines.AddRange(GenerateAnnotations(annotations.Except(annotationsToRemove)));

                if (lines.Count > 0)
                {
                    using (sb.Indent())
                    {
                        sb.AppendLine();
                        sb.Append("modelBuilder" + lines[0]);

                        using (sb.Indent())
                        {
                            foreach (var line in lines.Skip(1))
                            {
                                sb.AppendLine();
                                sb.Append(line);
                            }
                        }

                        sb.AppendLine(";");
                    }
                }

                using (sb.Indent())
                {

                    foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
                    {
                        _entityTypeBuilderInitialized = false;

                        GenerateEntityType(entityType, useDataAnnotations, sb);

                        if (_entityTypeBuilderInitialized)
                        {
                            sb.AppendLine("});");
                        }
                    }

                    foreach (var sequence in model.GetSequences())
                    {
                        GenerateSequence(sequence, sb);
                    }
                }

                sb.AppendLine();
                using (sb.Indent())
                {
                    sb.AppendLine("OnModelCreatingPartial(modelBuilder);");
                }
                sb.AppendLine("}");
                sb.AppendLine();
                sb.Append("partial void OnModelCreatingPartial(ModelBuilder modelBuilder);");
            }

            var onModelCreating = sb.ToString();
            TemplateData.Add("on-model-creating", onModelCreating);
        }

        private void GenerateDbSets(IModel model)
        {
            var dbSets = new List<Dictionary<string, object>>();

            foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
            {
                var transformedEntityName = EntityTypeTransformationService.TransformEntityName(entityType.Name);
                dbSets.Add(new Dictionary<string, object>
                {
                    { "set-property-type", transformedEntityName },
                    { "set-property-name", entityType.GetDbSetName() },
                    { "nullable-reference-types",  _options?.Value?.EnableNullableReferenceTypes == true }
                });
            }

            TemplateData.Add("dbsets", dbSets);
        }

        private void GenerateEntityTypeErrors(IModel model)
        {
            var entityTypeErrors = new List<Dictionary<string, object>>();

            foreach (var entityTypeError in model.GetEntityTypeErrors())
            {
                entityTypeErrors.Add(new Dictionary<string, object>
                {
                    { "entity-type-error", $"// {entityTypeError.Value} Please see the warning messages." },
                });
            }

            TemplateData.Add("entity-type-errors", entityTypeErrors);
        }

        private void InitializeEntityTypeBuilder(IEntityType entityType, IndentedStringBuilder sb)
        {
            if (!_entityTypeBuilderInitialized)
            {
                var transformedEntityName = EntityTypeTransformationService.TransformEntityName(entityType.Name);
                
                sb.AppendLine();
                sb.AppendLine($"modelBuilder.Entity<{transformedEntityName}>({EntityLambdaIdentifier} =>");
                sb.Append("{");
            }

            _entityTypeBuilderInitialized = true;
        }

        private void GenerateEntityType(IEntityType entityType, bool useDataAnnotations, IndentedStringBuilder sb)
        {
            GenerateKey(entityType.FindPrimaryKey(), entityType, useDataAnnotations, sb);

            var annotations = entityType.GetAnnotations().ToList();
            RemoveAnnotation(ref annotations, CoreAnnotationNames.ConstructorBinding);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.TableName);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.Comment);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.Schema);
            RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.DbSetName);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames2.ViewDefinition);

            var isView = entityType.FindAnnotation(RelationalAnnotationNames2.ViewDefinition) != null;
            if (!useDataAnnotations || isView)
            {
                GenerateTableName(entityType, sb);
            }

            var annotationsToRemove = new List<IAnnotation>();
            var lines = new List<string>();

            foreach (var annotation in annotations)
            {
                if (annotation.Value == null
                    || AnnotationCodeGenerator.IsHandledByConvention(entityType, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = AnnotationCodeGenerator.GenerateFluentApi(entityType, annotation);
                    if (methodCall != null)
                    {
                        lines.Add(CSharpHelper.Fragment(methodCall));
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(GenerateAnnotations(annotations.Except(annotationsToRemove)));

            if (entityType.GetComment() != null)
            {
                lines.Add(
                    $".{nameof(RelationalEntityTypeBuilderExtensions.HasComment)}" +
                    $"({CSharpHelper.Literal(entityType.GetComment())})");
            }

            AppendMultiLineFluentApi(entityType, lines, sb);

            foreach (var index in entityType.GetIndexes())
            {
                GenerateIndex(index, sb);
            }

            foreach (var property in entityType.GetProperties())
            {
                GenerateProperty(property, useDataAnnotations, sb);
            }

            foreach (var foreignKey in entityType.GetScaffoldForeignKeys(_options.Value))
            {
                GenerateRelationship(foreignKey, useDataAnnotations, sb);
            }
        }

        private void AppendMultiLineFluentApi(IEntityType entityType, IList<string> lines, IndentedStringBuilder sb)
        {
            if (lines.Count <= 0)
            {
                return;
            }

            InitializeEntityTypeBuilder(entityType, sb);

            using (sb.Indent())
            {
                sb.AppendLine();

                sb.Append(EntityLambdaIdentifier + lines[0]);

                using (sb.Indent())
                {
                    foreach (var line in lines.Skip(1))
                    {
                        sb.AppendLine();
                        sb.Append(line);
                    }
                }

                sb.AppendLine(";");
            }
        }

        private void GenerateKey(IKey key, IEntityType entityType, bool useDataAnnotations, IndentedStringBuilder sb)
        {
            if (key == null)
            {
                var line = new List<string>
                {
                    $".{nameof(EntityTypeBuilder.HasNoKey)}()"
                };

                AppendMultiLineFluentApi(entityType, line, sb);

                return;
            }

            var annotations = key.GetAnnotations().ToList();

            var explicitName = key.GetName() != key.GetDefaultName();
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.Name);

            if (key.Properties.Count == 1
                && annotations.Count == 0)
            {
                if (key is Key concreteKey
                    && key.Properties.SequenceEqual(
                        KeyDiscoveryConvention.DiscoverKeyProperties(
                            concreteKey.DeclaringEntityType,
                            concreteKey.DeclaringEntityType.GetProperties())))
                {
                    return;
                }

                if (!explicitName
                    && useDataAnnotations)
                {
                    return;
                }
            }

            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.HasKey)}(e => {GenerateLambdaToKey(key.Properties, "e", EntityTypeTransformationService.TransformPropertyName)})"
            };

            if (explicitName)
            {
                lines.Add(
                    $".{nameof(RelationalKeyBuilderExtensions.HasName)}" +
                    $"({CSharpHelper.Literal(key.GetName())})");
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (annotation.Value == null
                    || AnnotationCodeGenerator.IsHandledByConvention(key, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = AnnotationCodeGenerator.GenerateFluentApi(key, annotation);
                    if (methodCall != null)
                    {
                        lines.Add(CSharpHelper.Fragment(methodCall));
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(GenerateAnnotations(annotations.Except(annotationsToRemove)));

            AppendMultiLineFluentApi(key.DeclaringEntityType, lines, sb);
        }

        private void GenerateTableName(IEntityType entityType, IndentedStringBuilder sb)
        {
            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema();
            var defaultSchema = entityType.Model.GetDefaultSchema();

            var explicitSchema = schema != null && schema != defaultSchema;
            var explicitTable = explicitSchema || tableName != null && tableName != entityType.GetDbSetName();

            var isView = entityType.FindAnnotation(RelationalAnnotationNames2.ViewDefinition) != null;
            if (explicitTable || isView)
            {
                var parameterString = CSharpHelper.Literal(tableName);
                if (explicitSchema)
                {
                    parameterString += ", " + CSharpHelper.Literal(schema);
                }

                var lines = new List<string>
                {
                    $".{(isView ? nameof(RelationalEntityTypeBuilderExtensions.ToView) : nameof(RelationalEntityTypeBuilderExtensions.ToTable))}({parameterString})"
                };

                AppendMultiLineFluentApi(entityType, lines, sb);
            }
        }

        private void GenerateIndex(IIndex index, IndentedStringBuilder sb)
        {
            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.HasIndex)}(e => {GenerateLambdaToKey(index.Properties, "e", EntityTypeTransformationService.TransformPropertyName)})"
            };

            var annotations = index.GetAnnotations().ToList();

            if (!string.IsNullOrEmpty((string)index[RelationalAnnotationNames.Name]))
            {
                lines.Add(
                    $".{nameof(RelationalIndexBuilderExtensions.HasName)}" +
                    $"({CSharpHelper.Literal(index.GetName())})");
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.Name);
            }

            if (index.IsUnique)
            {
                lines.Add($".{nameof(IndexBuilder.IsUnique)}()");
            }

            if (index.GetFilter() != null)
            {
                lines.Add(
                    $".{nameof(RelationalIndexBuilderExtensions.HasFilter)}" +
                    $"({CSharpHelper.Literal(index.GetFilter())})");
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.Filter);
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (annotation.Value == null
                    || AnnotationCodeGenerator.IsHandledByConvention(index, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = AnnotationCodeGenerator.GenerateFluentApi(index, annotation);
                    if (methodCall != null)
                    {
                        lines.Add(CSharpHelper.Fragment(methodCall));
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(GenerateAnnotations(annotations.Except(annotationsToRemove)));

            AppendMultiLineFluentApi(index.DeclaringEntityType, lines, sb);
        }

        private void GenerateProperty(IProperty property, bool useDataAnnotations, IndentedStringBuilder sb)
        {
            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.Property)}(e => e.{EntityTypeTransformationService.TransformPropertyName(property.Name)})"
            };

            var annotations = property.GetAnnotations().ToList();

            RemoveAnnotation(ref annotations, RelationalAnnotationNames.ColumnName);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.ColumnType);
            RemoveAnnotation(ref annotations, CoreAnnotationNames.MaxLength);
            RemoveAnnotation(ref annotations, CoreAnnotationNames.TypeMapping);
            RemoveAnnotation(ref annotations, CoreAnnotationNames.Unicode);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.DefaultValue);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.DefaultValueSql);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.Comment);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.ComputedColumnSql);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.IsFixedLength);
            RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.ColumnOrdinal);

            if (!useDataAnnotations)
            {
                if (!property.IsNullable
                    && property.ClrType.IsNullableType()
                    && !property.IsPrimaryKey())
                {
                    lines.Add($".{nameof(PropertyBuilder.IsRequired)}()");
                }

                var columnName = property.GetColumnName();

                if (columnName != null
                    && columnName != property.Name)
                {
                    lines.Add(
                        $".{nameof(RelationalPropertyBuilderExtensions.HasColumnName)}" +
                        $"({CSharpHelper.Literal(columnName)})");
                }

                var columnType = property.GetConfiguredColumnType();

                if (columnType != null)
                {
                    lines.Add(
                        $".{nameof(RelationalPropertyBuilderExtensions.HasColumnType)}" +
                        $"({CSharpHelper.Literal(columnType)})");
                }

                var maxLength = property.GetMaxLength();

                if (maxLength.HasValue)
                {
                    lines.Add(
                        $".{nameof(PropertyBuilder.HasMaxLength)}" +
                        $"({CSharpHelper.Literal(maxLength.Value)})");
                }
            }

            if (property.IsUnicode() != null)
            {
                lines.Add(
                    $".{nameof(PropertyBuilder.IsUnicode)}" +
                    $"({(property.IsUnicode() == false ? "false" : "")})");
            }

            if (property.IsFixedLength())
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.IsFixedLength)}()");
            }

            if (property.GetDefaultValue() != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasDefaultValue)}" +
                    $"({CSharpHelper.UnknownLiteral(property.GetDefaultValue())})");
            }

            if (property.GetDefaultValueSql() != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasDefaultValueSql)}" +
                    $"({CSharpHelper.Literal(property.GetDefaultValueSql())})");
            }

            if (property.GetComment() != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasComment)}" +
                    $"({CSharpHelper.Literal(property.GetComment())})");
            }

            if (property.GetComputedColumnSql() != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasComputedColumnSql)}" +
                    $"({CSharpHelper.Literal(property.GetComputedColumnSql())})");
            }

            var valueGenerated = property.ValueGenerated;
            var isRowVersion = false;
            if (((IConventionProperty)property).GetValueGeneratedConfigurationSource().HasValue
                && RelationalValueGenerationConvention.GetValueGenerated(property) != valueGenerated)
            {
                string methodName;
                switch (valueGenerated)
                {
                    case ValueGenerated.OnAdd:
                        methodName = nameof(PropertyBuilder.ValueGeneratedOnAdd);
                        break;

                    case ValueGenerated.OnAddOrUpdate:
                        isRowVersion = property.IsConcurrencyToken;
                        methodName = isRowVersion
                            ? nameof(PropertyBuilder.IsRowVersion)
                            : nameof(PropertyBuilder.ValueGeneratedOnAddOrUpdate);
                        break;

                    case ValueGenerated.Never:
                        methodName = nameof(PropertyBuilder.ValueGeneratedNever);
                        break;

                    default:
                        methodName = "";
                        break;
                }

                lines.Add($".{methodName}()");
            }

            if (property.IsConcurrencyToken
                && !isRowVersion)
            {
                lines.Add($".{nameof(PropertyBuilder.IsConcurrencyToken)}()");
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (annotation.Value == null
                    || AnnotationCodeGenerator.IsHandledByConvention(property, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = AnnotationCodeGenerator.GenerateFluentApi(property, annotation);
                    if (methodCall != null)
                    {
                        lines.Add(CSharpHelper.Fragment(methodCall));
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(GenerateAnnotations(annotations.Except(annotationsToRemove)));

            switch (lines.Count)
            {
                case 1:
                    return;
                case 2:
                    lines = new List<string>
                    {
                        lines[0] + lines[1]
                    };
                    break;
            }

            AppendMultiLineFluentApi(property.DeclaringEntityType, lines, sb);
        }

        private void GenerateRelationship(IForeignKey foreignKey, bool useDataAnnotations, IndentedStringBuilder sb)
        {
            var canUseDataAnnotations = true;
            var annotations = foreignKey.GetAnnotations().ToList();

            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.HasOne)}(" + (foreignKey.DependentToPrincipal != null ? $"d => d.{EntityTypeTransformationService.TransformNavPropertyName(foreignKey.DependentToPrincipal.Name)}" : null) + ")",
                $".{(foreignKey.IsUnique ? nameof(ReferenceNavigationBuilder.WithOne) : nameof(ReferenceNavigationBuilder.WithMany))}"
                + $"(" + (foreignKey.PrincipalToDependent != null ? $"p => p.{EntityTypeTransformationService.TransformNavPropertyName(foreignKey.PrincipalToDependent.Name)}" : null) + ")"
            };

            if (!foreignKey.PrincipalKey.IsPrimaryKey())
            {
                canUseDataAnnotations = false;
                lines.Add(
                    $".{nameof(ReferenceReferenceBuilder.HasPrincipalKey)}"
                    + (foreignKey.IsUnique ? $"<{EntityTypeTransformationService.TransformPropertyName(((ITypeBase)foreignKey.PrincipalEntityType).DisplayName())}>" : "")
                    + $"(p => {GenerateLambdaToKey(foreignKey.PrincipalKey.Properties, "p", EntityTypeTransformationService.TransformNavPropertyName)})");
            }

            lines.Add(
                $".{nameof(ReferenceReferenceBuilder.HasForeignKey)}"
                + (foreignKey.IsUnique ? $"<{EntityTypeTransformationService.TransformPropertyName(((ITypeBase)foreignKey.DeclaringEntityType).DisplayName())}>" : "")
                + $"(d => {GenerateLambdaToKey(foreignKey.Properties, "d", EntityTypeTransformationService.TransformPropertyName)})");

            var defaultOnDeleteAction = foreignKey.IsRequired
                ? DeleteBehavior.Cascade
                : DeleteBehavior.ClientSetNull;

            if (foreignKey.DeleteBehavior != defaultOnDeleteAction)
            {
                canUseDataAnnotations = false;
                lines.Add(
                    $".{nameof(ReferenceReferenceBuilder.OnDelete)}" +
                    $"({CSharpHelper.Literal(foreignKey.DeleteBehavior)})");
            }

            if (!string.IsNullOrEmpty((string)foreignKey[RelationalAnnotationNames.Name]))
            {
                canUseDataAnnotations = false;
                lines.Add(
                    $".HasConstraintName" +
                    $"({CSharpHelper.Literal(foreignKey.GetConstraintName())})");
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.Name);
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (annotation.Value == null
                    || AnnotationCodeGenerator.IsHandledByConvention(foreignKey, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = AnnotationCodeGenerator.GenerateFluentApi(foreignKey, annotation);
                    if (methodCall != null)
                    {
                        canUseDataAnnotations = false;
                        lines.Add(CSharpHelper.Fragment(methodCall));
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(GenerateAnnotations(annotations.Except(annotationsToRemove)));

            if (!useDataAnnotations
                || !canUseDataAnnotations)
            {
                AppendMultiLineFluentApi(foreignKey.DeclaringEntityType, lines, sb);
            }
        }

        private void GenerateSequence(ISequence sequence, IndentedStringBuilder sb)
        {
            var methodName = nameof(RelationalModelBuilderExtensions.HasSequence);

            if (sequence.ClrType != Sequence.DefaultClrType)
            {
                methodName += $"<{CSharpHelper.Reference(sequence.ClrType)}>";
            }

            var parameters = CSharpHelper.Literal(sequence.Name);

            if (!string.IsNullOrEmpty(sequence.Schema)
                && sequence.Model.GetDefaultSchema() != sequence.Schema)
            {
                parameters += $", {CSharpHelper.Literal(sequence.Schema)}";
            }

            var lines = new List<string>
            {
                $"modelBuilder.{methodName}({parameters})"
            };

            if (sequence.StartValue != Sequence.DefaultStartValue)
            {
                lines.Add($".{nameof(SequenceBuilder.StartsAt)}({sequence.StartValue})");
            }

            if (sequence.IncrementBy != Sequence.DefaultIncrementBy)
            {
                lines.Add($".{nameof(SequenceBuilder.IncrementsBy)}({sequence.IncrementBy})");
            }

            if (sequence.MinValue != Sequence.DefaultMinValue)
            {
                lines.Add($".{nameof(SequenceBuilder.HasMin)}({sequence.MinValue})");
            }

            if (sequence.MaxValue != Sequence.DefaultMaxValue)
            {
                lines.Add($".{nameof(SequenceBuilder.HasMax)}({sequence.MaxValue})");
            }

            if (sequence.IsCyclic != Sequence.DefaultIsCyclic)
            {
                lines.Add($".{nameof(SequenceBuilder.IsCyclic)}()");
            }

            if (lines.Count == 2)
            {
                lines = new List<string>
                {
                    lines[0] + lines[1]
                };
            }

            sb.AppendLine();
            sb.Append(lines[0]);

            using (sb.Indent())
            {
                foreach (var line in lines.Skip(1))
                {
                    sb.AppendLine();
                    sb.Append(line);
                }
            }

            sb.AppendLine(";");
        }

        private static string GenerateLambdaToKey(
            IReadOnlyList<IProperty> properties,
            string lambdaIdentifier,
            Func<string, string> nameTransform)
        {
            return properties.Count <= 0
                ? ""
                : properties.Count == 1
                ? $"{lambdaIdentifier}.{nameTransform(properties[0].Name)}"
                : $"new {{ {string.Join(", ", properties.Select(p => lambdaIdentifier + "." + nameTransform(p.Name)))} }}";
        }

        private static void RemoveAnnotation(ref List<IAnnotation> annotations, string annotationName)
            => annotations.Remove(annotations.SingleOrDefault(a => a.Name == annotationName));

        private IList<string> GenerateAnnotations(IEnumerable<IAnnotation> annotations)
            => annotations.Select(GenerateAnnotation).ToList();

        private string GenerateAnnotation(IAnnotation annotation)
            => $".HasAnnotation({CSharpHelper.Literal(annotation.Name)}, " +
               $"{CSharpHelper.UnknownLiteral(annotation.Value)})";
    }
}
