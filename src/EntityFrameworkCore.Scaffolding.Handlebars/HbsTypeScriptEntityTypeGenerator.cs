using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

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
        /// Indicates if nullable reference types should be used.
        /// </summary>
        protected bool UseNullableReferenceTypes { get; private set; }

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
        /// <param name="useNullableReferenceTypes">If true use nullable reference types.</param>
        /// <returns>Generated entity type.</returns>
        public virtual string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations, bool useNullableReferenceTypes)
        {
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(@namespace, nameof(@namespace));

            UseNullableReferenceTypes = useNullableReferenceTypes;
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

            var navigations = new List<INavigationBase>();
            IEnumerable<INavigationBase> sortedNavigations = entityType.GetScaffoldNavigations(_options.Value)
                .OrderBy(n => n.IsOnDependent ? 0 : 1)
                .ThenBy(n => n.IsCollection ? 1 : 0)
                .Distinct();
            IEnumerable<INavigationBase> sortedSkipNavigations = entityType.GetScaffoldSkipNavigations(_options.Value)
                .OrderBy(n => n.IsOnDependent ? 0 : 1)
                .ThenBy(n => n.IsCollection ? 1 : 0)
                .Distinct();
            navigations.AddRange(sortedNavigations);
            navigations.AddRange(sortedSkipNavigations);

            if (navigations.Any())
            {
                var imports = new List<Dictionary<string, object>>();
                foreach (var navigation in navigations)
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
        protected virtual void GenerateClass(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var transformedEntityName = EntityTypeTransformationService.TransformTypeEntityName(entityType, entityType.Name);

            TemplateData.Add("comment", entityType.GetComment());
            TemplateData.Add("class", transformedEntityName);

            GenerateConstructor(entityType);
            GenerateProperties(entityType);
            GenerateNavigationProperties(entityType);
            GenerateSkipNavigationProperties(entityType);
        }

        /// <summary>
        /// Generate entity type constructor.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateConstructor(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var collectionNavigations = entityType.GetNavigations().Where(n => n.IsCollection).ToList();

            if (collectionNavigations.Count > 0)
            {
                var lines = new List<Dictionary<string, object>>();

                foreach (var navigation in collectionNavigations)
                {
                    var navPropertyType = EntityTypeTransformationService.TransformTypeEntityName(navigation.TargetEntityType, navigation.TargetEntityType.Name);
                    lines.Add(new Dictionary<string, object>
                    {
                        { "property-name", navigation.Name },
                        { "property-type", navPropertyType },
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
        protected virtual void GenerateProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var properties = new List<Dictionary<string, object>>();

            foreach (var property in entityType.GetProperties().OrderBy(p => p.GetColumnOrder()))
            {
                properties.Add(new Dictionary<string, object>
                {
                    { "property-type", TypeScriptHelper.TypeName(property.ClrType) },
                    { "property-name",  TypeScriptHelper.ToCamelCase(property.Name) },
                    { "property-annotations",  new List<Dictionary<string, object>>() },
                    { "property-comment", property.GetComment() },
                    { "property-isnullable", property.IsNullable },
                    { "property-isenum", false },
                    { "property-default-enum", null },
                    { "nullable-reference-types", UseNullableReferenceTypes }
                });
            }

            var transformedProperties = EntityTypeTransformationService.TransformProperties(entityType, properties);

            TemplateData.Add("properties", transformedProperties);
        }

        /// <summary>
        /// Generate entity type navigation properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateNavigationProperties(IEntityType entityType)
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
                    var navPropertyType = EntityTypeTransformationService.TransformTypeEntityName(navigation.TargetEntityType, navigation.TargetEntityType.Name);
                    navProperties.Add(new Dictionary<string, object>
                    {
                        { "nav-property-collection", navigation.IsCollection },
                        { "nav-property-type", navPropertyType },
                        { "nav-property-name", TypeScriptHelper.ToCamelCase(navigation.Name) },
                        { "nav-property-annotations", new List<Dictionary<string, object>>() },
                        { "nav-property-isnullable", false },
                        { "nullable-reference-types",  UseNullableReferenceTypes }
                    });
                }

                var transformedNavProperties = EntityTypeTransformationService.TransformNavigationProperties(entityType, navProperties);

                TemplateData.Add("nav-properties", transformedNavProperties);
            }
        }

        /// <summary>
        /// Generate entity type skip navigation properties.
        /// </summary>
        /// <param name="entityType">Represents an entity type in an <see cref="T:Microsoft.EntityFrameworkCore.Metadata.IModel" />.</param>
        protected virtual void GenerateSkipNavigationProperties(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var sortedNavigations = entityType.GetSkipNavigations()
                .OrderBy(n => n.IsOnDependent ? 0 : 1)
                .ThenBy(n => n.IsCollection ? 1 : 0);

            if (sortedNavigations.Any())
            {
                var navProperties = new List<Dictionary<string, object>>();

                foreach (var navigation in sortedNavigations)
                {
                    var navPropertyType = EntityTypeTransformationService.TransformTypeEntityName(navigation.TargetEntityType, navigation.TargetEntityType.Name);
                    navProperties.Add(new Dictionary<string, object>
                    {
                        { "nav-property-collection", navigation.IsCollection },
                        { "nav-property-type", navPropertyType },
                        { "nav-property-name", TypeScriptHelper.ToCamelCase(navigation.Name) },
                        { "nav-property-annotations", new List<Dictionary<string, object>>() },
                        { "nav-property-isnullable", false },
                        { "nullable-reference-types",  UseNullableReferenceTypes }
                    });
                }

                var transformedNavProperties = EntityTypeTransformationService.TransformNavigationProperties(entityType, navProperties);
                if (TemplateData.TryGetValue("nav-properties", out var navProps)
                    && navProps is List<Dictionary<string, object>> existingNavProps)
                    transformedNavProperties.ForEach(item => existingNavProps.Add(item));
                else
                    TemplateData.Add("nav-properties", transformedNavProperties);
            }
        }
    }
}
