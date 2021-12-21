using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Design
{
    /// <summary>
    /// Helper methods for configuring code generation for Entity Framework Core using Handlebars templates.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Registers the Handlebars scaffolding generator as a service in the <see cref="IServiceCollection" />.
        ///         This allows you to customize generated DbContext and entity type classes by modifying the Handlebars 
        ///         templates in the CodeTemplates folder.
        ///     </para>
        ///     <para>
        ///         Has <paramref name="options" /> that allow you to choose whether to generate only the DbContext class, 
        ///         only entity type classes, or both DbContext and entity type classes (the default).
        ///     </para>
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="options">Options for reverse engineering classes from an existing database.</param>
        /// <param name="language">Language option.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddHandlebarsScaffolding(this IServiceCollection services,
            ReverseEngineerOptions options = ReverseEngineerOptions.DbContextAndEntities,
            LanguageOptions language = LanguageOptions.CSharp)
        {
            return services.AddHandlebarsScaffolding(scaffoldingOptions =>
            {
                scaffoldingOptions.ReverseEngineerOptions = options;
                scaffoldingOptions.LanguageOptions = language;
            });
        }

        /// <summary>
        ///     <para>
        ///         Registers the Handlebars scaffolding generator as a service in the <see cref="IServiceCollection" />.
        ///         This allows you to customize generated DbContext and entity type classes by modifying the Handlebars 
        ///         templates in the CodeTemplates folder.
        ///     </para>
        ///     <para>
        ///         Has <paramref name="configureOptions" /> that allow you to choose whether to generate only the DbContext class, 
        ///         only entity type classes, or both DbContext and entity type classes (the default).
        ///         It also allows you to exclude tables from the generation.
        ///         This can be useful when placing model classes in a separate class library.
        ///     </para>
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="configureOptions">Method for configuring options for reverse engineering classes from an existing database.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddHandlebarsScaffolding(this IServiceCollection services,
            Action<HandlebarsScaffoldingOptions> configureOptions)
        {
            var scaffoldingOptions = new HandlebarsScaffoldingOptions();
            if (configureOptions == null)
                configureOptions = options => options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;
            configureOptions(scaffoldingOptions);
            services.Configure(configureOptions);

            Type dbContextGeneratorImpl;
            var dbContextGeneratorType = typeof(ICSharpDbContextGenerator);
            if (scaffoldingOptions.ReverseEngineerOptions == ReverseEngineerOptions.DbContextOnly
                || scaffoldingOptions.ReverseEngineerOptions == ReverseEngineerOptions.DbContextAndEntities)
                dbContextGeneratorImpl = typeof(HbsCSharpDbContextGenerator);
            else
                dbContextGeneratorImpl = typeof(NullCSharpDbContextGenerator);
            services.AddSingleton(dbContextGeneratorType, dbContextGeneratorImpl);

            Type entityGeneratorImpl;
            var entityGeneratorType = typeof(ICSharpEntityTypeGenerator);
            if (scaffoldingOptions.ReverseEngineerOptions == ReverseEngineerOptions.EntitiesOnly
                || scaffoldingOptions.ReverseEngineerOptions == ReverseEngineerOptions.DbContextAndEntities)
            {
                if (scaffoldingOptions.LanguageOptions == LanguageOptions.TypeScript)
                    entityGeneratorImpl = typeof(HbsTypeScriptEntityTypeGenerator);
                else
                    entityGeneratorImpl = typeof(HbsCSharpEntityTypeGenerator);
            }
            else
            {
                entityGeneratorImpl = typeof(NullCSharpEntityTypeGenerator);
            }

            services.AddSingleton(entityGeneratorType, entityGeneratorImpl);
            services.AddSingleton<IContextTransformationService, HbsContextTransformationService>();

            if (scaffoldingOptions.LanguageOptions == LanguageOptions.TypeScript)
            {
                services.AddSingleton<ITypeScriptHelper, TypeScriptHelper>();
                services.AddSingleton<IModelCodeGenerator, HbsTypeScriptModelGenerator>();
                services.AddSingleton<ITemplateLanguageService, TypeScriptTemplateLanguageService>();
            }
            else
            {
                services.AddSingleton<IModelCodeGenerator, HbsCSharpModelGenerator>();
                services.AddSingleton<ITemplateLanguageService, CSharpTemplateLanguageService>();
            }

            if (scaffoldingOptions.EmbeddedTemplatesAssembly != null)
            {
                services.AddSingleton<ITemplateFileService>(new EmbeddedResourceTemplateFileService(
                    scaffoldingOptions.EmbeddedTemplatesAssembly, scaffoldingOptions.EmbeddedTemplatesNamespace));
            }
            else
            {
                services.AddSingleton<ITemplateFileService, FileSystemTemplateFileService>();
            }

            services.AddSingleton<IDbContextTemplateService, HbsDbContextTemplateService>();
            services.AddSingleton<IEntityTypeTemplateService, HbsEntityTypeTemplateService>();
            services.AddSingleton<IReverseEngineerScaffolder, HbsReverseEngineerScaffolder>();
            services.AddSingleton<IEntityTypeTransformationService, HbsEntityTypeTransformationService>();
            services.AddSingleton<IHbsHelperService, HbsHelperService>(provider =>
            {
                var helpers = new Dictionary<string, Action<EncodedTextWriter, Context, Arguments>>
                {
                    {Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                };
                return new HbsHelperService(helpers);
            });
            services.AddSingleton<IHbsBlockHelperService, HbsBlockHelperService>(provider =>
            {
                var helpers = new Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>>();
                return new HbsBlockHelperService(helpers);
            });
            return services;
        }

        /// <summary>
        /// Register Handlebars helpers.
        ///     <para>
        ///         Note: You must first call AddHandlebarsScaffolding before calling AddHandlebarsHelpers.
        ///     </para>
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="handlebarsHelpers">Handlebars helpers.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddHandlebarsHelpers(this IServiceCollection services,
            params (string helperName, Action<EncodedTextWriter, Context, Arguments> helperFunction)[] handlebarsHelpers)
        {
            services.AddSingleton<IHbsHelperService>(provider =>
            {
                var helpers = new Dictionary<string, Action<EncodedTextWriter, Context, Arguments>>
                {
                    {Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                };
                handlebarsHelpers.ToList().ForEach(h => helpers.Add(h.helperName, h.helperFunction));
                return new HbsHelperService(helpers);
            });
            return services;
        }

        /// <summary>
        /// Register Handlebars block helpers.
        ///     <para>
        ///         Note: You must first call AddHandlebarsScaffolding before calling AddHandlebarsBlockHelpers.
        ///     </para>        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="handlebarsBlockHelpers">Handlebars block helpers.</param>
        /// <returns></returns>
        public static IServiceCollection AddHandlebarsBlockHelpers(this IServiceCollection services,
            params (string helperName, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments> helperFunction)[] handlebarsBlockHelpers)
        {
            services.AddSingleton<IHbsBlockHelperService>(provider =>
            {
                var helpers = new Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>>();
                handlebarsBlockHelpers.ToList().ForEach(h => helpers.Add(h.helperName, h.helperFunction));
                return new HbsBlockHelperService(helpers);
            });
            return services;
        }

        /// <summary>
        /// Register Handlebars transformers.
        ///     <para>
        ///         Note: You must first call AddHandlebarsScaffolding before calling AddHandlebarsTransformers.
        ///     </para>
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="entityTypeNameTransformer">Entity name transformer.</param>
        /// <param name="entityFileNameTransformer">Entity file name transformer.</param>
        /// <param name="constructorTransformer"></param>
        /// <param name="propertyTransformer">Property name transformer.</param>
        /// <param name="navPropertyTransformer">Navigation property name transformer.</param>
        /// <param name="contextFileNameTransformer">Context file name transformer.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddHandlebarsTransformers(this IServiceCollection services,
            Func<string, string> entityTypeNameTransformer = null,
            Func<string, string> entityFileNameTransformer = null,
            Func<EntityPropertyInfo, EntityPropertyInfo> constructorTransformer = null,
            Func<EntityPropertyInfo, EntityPropertyInfo> propertyTransformer = null,
            Func<EntityPropertyInfo, EntityPropertyInfo> navPropertyTransformer = null,
            Func<string, string> contextFileNameTransformer = null)
        {
            services.AddSingleton<IEntityTypeTransformationService>(provider =>
                new HbsEntityTypeTransformationService(
                    entityTypeNameTransformer,
                    entityFileNameTransformer,
                    constructorTransformer,
                    propertyTransformer,
                    navPropertyTransformer));
            services.AddSingleton<IContextTransformationService>(provider =>
                new HbsContextTransformationService(contextFileNameTransformer));
            return services;
        }

        /// <summary>
        /// Register Handlebars transformers.
        ///     <para>
        ///         Note: You must first call AddHandlebarsScaffolding before calling AddHandlebarsTransformers.
        ///         This overload surfaces IEntityType in the transformers.
        ///     </para>
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <param name="entityTypeNameTransformer">Entity name transformer.</param>
        /// <param name="entityFileNameTransformer">Entity file name transformer.</param>
        /// <param name="constructorTransformer"></param>
        /// <param name="propertyTransformer">Property name transformer.</param>
        /// <param name="navPropertyTransformer">Navigation property name transformer.</param>
        /// <param name="contextFileNameTransformer">Context file name transformer.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddHandlebarsTransformers2(this IServiceCollection services,
            Func<string, string> entityTypeNameTransformer = null,
            Func<string, string> entityFileNameTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> constructorTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> propertyTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> navPropertyTransformer = null,
            Func<string, string> contextFileNameTransformer = null)
        {
            services.AddSingleton<IEntityTypeTransformationService>(provider =>
                new HbsEntityTypeTransformationService2(
                    entityTypeNameTransformer,
                    entityFileNameTransformer,
                    constructorTransformer,
                    propertyTransformer,
                    navPropertyTransformer));
            services.AddSingleton<IContextTransformationService>(provider =>
                new HbsContextTransformationService(contextFileNameTransformer));
            return services;
        }
    }
}