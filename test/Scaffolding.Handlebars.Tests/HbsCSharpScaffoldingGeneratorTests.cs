using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Scaffolding.Handlebars.Tests.Helpers;
using Xunit;
using Constants = Scaffolding.Handlebars.Tests.Helpers.Constants;

namespace Scaffolding.Handlebars.Tests
{
    [Collection("NorthwindDbContext")]
    public partial class HbsCSharpScaffoldingGeneratorTests
    {
        private NorthwindDbContextFixture Fixture { get; }

        private InputFile ContextClassTemplate { get; }
        private InputFile ContextImportsTemplate { get; }
        private InputFile ContextDbSetsTemplate { get; }
        private InputFile EntityClassTemplate { get; }
        private InputFile EntityImportsTemplate { get; }
        private InputFile EntityCtorTemplate { get; }
        private InputFile EntityPropertiesTemplate { get; }

        public HbsCSharpScaffoldingGeneratorTests(NorthwindDbContextFixture fixture)
        {
            Fixture = fixture;
            Fixture.Initialize(useInMemory: false);

            var projectRootDir = Path.Combine("..", "..", "..", "..", "..");

            var contextTemplatesVirtualPath =
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.ContextFolder}";
            var contextPartialsVirtualPath = contextTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var contextTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder,
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.ContextFolder);
            var contextPartialTemplatesPath = Path.Combine(contextTemplatesPath, Constants.Templates.PartialsFolder);

            var entityTemplatesVirtualPath =
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.EntityTypeFolder}";
            var entityPartialsVirtualPath = entityTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var entityTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder, 
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.EntityTypeFolder);
            var entityPartialTemplatesPath = Path.Combine(entityTemplatesPath, Constants.Templates.PartialsFolder);

            ContextClassTemplate = new InputFile
            {
                Directory = contextTemplatesVirtualPath,
                File = Constants.Templates.ContextClassFile,
                Contents = File.ReadAllText(Path.Combine(contextTemplatesPath, Constants.Templates.ContextClassFile))
            };
            ContextImportsTemplate = new InputFile
            {
                Directory = contextPartialsVirtualPath,
                File = Constants.Templates.ContextImportsFile,
                Contents = File.ReadAllText(Path.Combine(contextPartialTemplatesPath, Constants.Templates.ContextImportsFile))
            };
            ContextDbSetsTemplate = new InputFile
            {
                Directory = contextPartialsVirtualPath,
                File = Constants.Templates.ContextDbSetsFile,
                Contents = File.ReadAllText(Path.Combine(contextPartialTemplatesPath, Constants.Templates.ContextDbSetsFile))
            };

            EntityClassTemplate = new InputFile
            {
                Directory = entityTemplatesVirtualPath,
                File = Constants.Templates.EntityClassFile,
                Contents = File.ReadAllText(Path.Combine(entityTemplatesPath, Constants.Templates.EntityClassFile))
            };
            EntityImportsTemplate = new InputFile
            {
                Directory = entityPartialsVirtualPath,
                File = Constants.Templates.EntityImportsFile,
                Contents = File.ReadAllText(Path.Combine(entityPartialTemplatesPath, Constants.Templates.EntityImportsFile))
            };
            EntityCtorTemplate = new InputFile
            {
                Directory = entityPartialsVirtualPath,
                File = Constants.Templates.EntityCtorFile,
                Contents = File.ReadAllText(Path.Combine(entityPartialTemplatesPath, Constants.Templates.EntityCtorFile))
            };
            EntityPropertiesTemplate = new InputFile
            {
                Directory = entityPartialsVirtualPath,
                File = Constants.Templates.EntityPropertiesFile,
                Contents = File.ReadAllText(Path.Combine(entityPartialTemplatesPath, Constants.Templates.EntityPropertiesFile))
            };
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WriteCode_Should_Generate_Context_File(bool useDataAnnotations)
        {
            // Act
            Dictionary<string, string> files = ReverseEngineerFiles(ReverseEngineerOptions.DbContextOnly, useDataAnnotations);

            // Assert
            object expectedContext = useDataAnnotations
                ? ExpectedContextsWithAnnotations.ContextClass
                : ExpectedContexts.ContextClass;

            var context = files[Constants.Files.DbContextFile];

            Assert.Equal(expectedContext, context);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WriteCode_Should_Generate_Entity_Files(bool useDataAnnotations)
        {
            // Act
            Dictionary<string, string> files = ReverseEngineerFiles(ReverseEngineerOptions.EntitiesOnly, useDataAnnotations);

            // Assert
            var category = files[Constants.Files.CategoryFile];
            var product = files[Constants.Files.ProductFile];

            object expectedCategory = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.CategoryClass
                : ExpectedEntities.CategoryClass;
            object expectedProduct = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.ProductClass
                : ExpectedEntities.ProductClass;

            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        private Dictionary<string, string> ReverseEngineerFiles(ReverseEngineerOptions options, bool useDataAnnotations)
        {
            var fileService = new InMemoryTemplateFileService();
            fileService.InputFiles(ContextClassTemplate, ContextImportsTemplate, ContextDbSetsTemplate,
                EntityClassTemplate, EntityImportsTemplate, EntityCtorTemplate, EntityPropertiesTemplate);

            var services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<IDbContextTemplateService, HbsDbContextTemplateService>()
                .AddSingleton<IEntityTypeTemplateService, HbsEntityTypeTemplateService>()
                .AddSingleton<ITemplateFileService>(fileService)
                .AddSingleton<IModelCodeGenerator, HbsCSharpModelGenerator>()
                .AddSingleton(provider =>
                {
                    ICSharpDbContextGenerator contextGenerator = new HbsCSharpDbContextGenerator(
#pragma warning disable 618
                        provider.GetRequiredService<IEnumerable<IScaffoldingProviderCodeGenerator>>(),
#pragma warning restore 618
                        provider.GetRequiredService<IEnumerable<IProviderConfigurationCodeGenerator>>(),
                        provider.GetRequiredService<IAnnotationCodeGenerator>(),
                        provider.GetRequiredService<IDbContextTemplateService>(),
                        provider.GetRequiredService<ICSharpHelper>());
                    return options == ReverseEngineerOptions.DbContextOnly ||
                           options == ReverseEngineerOptions.DbContextAndEntities
                        ? contextGenerator
                        : new NullCSharpDbContextGenerator();
                })
                .AddSingleton(provider =>
                {
                    ICSharpEntityTypeGenerator entityGenerator = new HbsCSharpEntityTypeGenerator(
                        provider.GetRequiredService<IEntityTypeTemplateService>(),
                        provider.GetRequiredService<ICSharpHelper>());
                    return options == ReverseEngineerOptions.EntitiesOnly ||
                           options == ReverseEngineerOptions.DbContextAndEntities
                        ? entityGenerator
                        : new NullCSharpEntityTypeGenerator();
                })
                .AddSingleton<IHbsHelperService>(provider =>
                new HbsHelperService(new Dictionary<string, Action<TextWriter, object, object[]>>
                {
                    {EntityFrameworkCore.Scaffolding.Handlebars.Helpers.Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                }));

            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
            var scaffolder = services
                .BuildServiceProvider()
                .GetRequiredService<IReverseEngineerScaffolder>();

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: Constants.Connections.SqlServerConnection,
                tables: Enumerable.Empty<string>(),
                schemas: Enumerable.Empty<string>(),
                @namespace: Constants.Parameters.RootNamespace,
                language: "C#",
                contextName: Constants.Parameters.ContextName,
                modelOptions: new ModelReverseEngineerOptions(),
                contextDir: Constants.Parameters.ProjectPath,
                codeOptions: new ModelCodeGenerationOptions { UseDataAnnotations = useDataAnnotations });

            var generatedFiles = new Dictionary<string, string>();

            if (options == ReverseEngineerOptions.DbContextOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                generatedFiles.Add(Constants.Files.DbContextFile, model.ContextFile.Code);
            }

            if (options == ReverseEngineerOptions.EntitiesOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                generatedFiles.Add(Constants.Files.CategoryFile, model.AdditionalFiles[0].Code);
                generatedFiles.Add(Constants.Files.ProductFile, model.AdditionalFiles[1].Code);
            }

            return generatedFiles;
        }
    }
}