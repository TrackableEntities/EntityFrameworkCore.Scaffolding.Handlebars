using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        private InputFile ContextCtorTemplate { get; }
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
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.CSharpTemplateDirectories.ContextFolder}";
            var contextPartialsVirtualPath = contextTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var contextTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder,
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.CSharpTemplateDirectories.ContextFolder);
            var contextPartialTemplatesPath = Path.Combine(contextTemplatesPath, Constants.Templates.PartialsFolder);

            var entityTemplatesVirtualPath =
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.CSharpTemplateDirectories.EntityTypeFolder}";
            var entityPartialsVirtualPath = entityTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var entityTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder,
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.CSharpTemplateDirectories.EntityTypeFolder);
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
            ContextCtorTemplate = new InputFile
            {
                Directory = contextPartialsVirtualPath,
                File = Constants.Templates.ContextCtorFile,
                Contents = File.ReadAllText(Path.Combine(contextPartialTemplatesPath, Constants.Templates.ContextCtorFile))
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
        [InlineData(false, false, "en-US")]
        [InlineData(true, false, "en-US")]
        [InlineData(false, false, "tr-TR")]
        [InlineData(true, false, "tr-TR")]
        [InlineData(false, true, "en-US")]
        [InlineData(true, true, "en-US")]
        [InlineData(false, true, "tr-TR")]
        [InlineData(true, true, "tr-TR")]

        public void WriteCode_Should_Generate_Context_File(bool useDataAnnotations, bool usePluralizer, string culture)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var options = ReverseEngineerOptions.DbContextOnly;
            var scaffolder = CreateScaffolder(options, usePluralizer);

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: Constants.Connections.SqlServerConnection,
                databaseOptions: new DatabaseModelFactoryOptions(),
                modelOptions: new ModelReverseEngineerOptions(),
                codeOptions: new ModelCodeGenerationOptions
                {
                    ContextNamespace = Constants.Parameters.RootNamespace,
                    ModelNamespace = Constants.Parameters.RootNamespace,
                    ContextName = Constants.Parameters.ContextName,
                    ContextDir = Constants.Parameters.ProjectPath,
                    UseDataAnnotations = useDataAnnotations,
                    Language = "C#",
                });

            // Assert
            var files = GetGeneratedFiles(model, options);
            object expectedContext;
            if (!usePluralizer)
            {
                expectedContext = useDataAnnotations
                    ? ExpectedContextsWithAnnotations.ContextClass
                    : ExpectedContexts.ContextClass;
            }
            else
            {
                expectedContext = useDataAnnotations
                    ? ExpectedContextsWithAnnotationsPluralized.ContextClass
                    : ExpectedContextsPluralized.ContextClass;
            }
            var context = files[Constants.Files.CSharpFiles.DbContextFile];

            Assert.Equal(expectedContext, context);
        }

        [Theory]
        [InlineData(false, false, "en-US")]
        [InlineData(true, false, "en-US")]
        [InlineData(false, false, "tr-TR")]
        [InlineData(true, false, "tr-TR")]
        [InlineData(false, true, "en-US")]
        [InlineData(true, true, "en-US")]
        [InlineData(false, true, "tr-TR")]
        [InlineData(true, true, "tr-TR")]

        public void WriteCode_Should_Generate_Entity_Files(bool useDataAnnotations, bool usePluralizer, string culture)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var options = ReverseEngineerOptions.EntitiesOnly;
            var scaffolder = CreateScaffolder(options, usePluralizer);

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: Constants.Connections.SqlServerConnection,
                databaseOptions: new DatabaseModelFactoryOptions(),
                modelOptions: new ModelReverseEngineerOptions(),
                codeOptions: new ModelCodeGenerationOptions
                {
                    ContextNamespace = Constants.Parameters.RootNamespace,
                    ModelNamespace = Constants.Parameters.RootNamespace,
                    ContextName = Constants.Parameters.ContextName,
                    ContextDir = Constants.Parameters.ProjectPath,
                    UseDataAnnotations = useDataAnnotations,
                    Language = "C#",
                });

            // Assert
            var files = GetGeneratedFiles(model, options);
            var category = files[Constants.Files.CSharpFiles.CategoryFile];
            var product = files[Constants.Files.CSharpFiles.ProductFile];

            object expectedCategory;
            object expectedProduct;
            if (!usePluralizer)
            {
                expectedCategory = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotations.CategoryClass
                    : ExpectedEntities.CategoryClass;
                expectedProduct = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotations.ProductClass
                    : ExpectedEntities.ProductClass;
            }
            else
            {
                expectedCategory = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotationsPluralized.CategoryClass
                    : ExpectedEntitiesPluralized.CategoryClass;
                expectedProduct = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotationsPluralized.ProductClass
                    : ExpectedEntitiesPluralized.ProductClass;
            }
            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        [Theory]
        [InlineData(false, false, "en-US")]
        [InlineData(true, false, "en-US")]
        [InlineData(false, false, "tr-TR")]
        [InlineData(true, false, "tr-TR")]
        [InlineData(false, true, "en-US")]
        [InlineData(true, true, "en-US")]
        [InlineData(false, true, "tr-TR")]
        [InlineData(true, true, "tr-TR")]
        public void WriteCode_Should_Generate_Context_and_Entity_Files(bool useDataAnnotations, bool usePluralizer, string culture)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var options = ReverseEngineerOptions.DbContextAndEntities;
            var scaffolder = CreateScaffolder(options, usePluralizer);

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: Constants.Connections.SqlServerConnection,
                databaseOptions: new DatabaseModelFactoryOptions(),
                modelOptions: new ModelReverseEngineerOptions(),
                codeOptions: new ModelCodeGenerationOptions
                {
                    ContextNamespace = Constants.Parameters.RootNamespace,
                    ModelNamespace = Constants.Parameters.RootNamespace,
                    ContextName = Constants.Parameters.ContextName,
                    ContextDir = Constants.Parameters.ProjectPath,
                    UseDataAnnotations = useDataAnnotations,
                    Language = "C#",
                });

            // Assert
            var files = GetGeneratedFiles(model, options);
            object expectedContext;
            object expectedCategory;
            object expectedProduct;
            if (!usePluralizer)
            {
                expectedContext = useDataAnnotations
                    ? ExpectedContextsWithAnnotations.ContextClass
                    : ExpectedContexts.ContextClass;
                expectedCategory = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotations.CategoryClass
                    : ExpectedEntities.CategoryClass;
                expectedProduct = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotations.ProductClass
                    : ExpectedEntities.ProductClass;
            }
            else
            {
                expectedContext = useDataAnnotations
                    ? ExpectedContextsWithAnnotationsPluralized.ContextClass
                    : ExpectedContextsPluralized.ContextClass;
                expectedCategory = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotationsPluralized.CategoryClass
                    : ExpectedEntitiesPluralized.CategoryClass;
                expectedProduct = useDataAnnotations
                    ? ExpectedEntitiesWithAnnotationsPluralized.ProductClass
                    : ExpectedEntitiesPluralized.ProductClass;
            }
            var context = files[Constants.Files.CSharpFiles.DbContextFile];
            var category = files[Constants.Files.CSharpFiles.CategoryFile];
            var product = files[Constants.Files.CSharpFiles.ProductFile];

            Assert.Equal(expectedContext, context);
            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        [Fact]
        public void Save_Should_Write_Context_File()
        {
            using (var directory = new TempDirectory())
            {
                // Arrange
                var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextOnly, false);
                var model = scaffolder.ScaffoldModel(
                    connectionString: Constants.Connections.SqlServerConnection,
                    databaseOptions: new DatabaseModelFactoryOptions(),
                    modelOptions: new ModelReverseEngineerOptions(),
                    codeOptions: new ModelCodeGenerationOptions
                    {
                        ContextNamespace = Constants.Parameters.RootNamespace,
                        ModelNamespace = Constants.Parameters.RootNamespace,
                        ContextName = Constants.Parameters.ContextName,
                        ContextDir = Path.Combine(directory.Path, "Contexts"),
                        UseDataAnnotations = false,
                        Language = "C#",
                    });

                // Act
                var result = scaffolder.Save(model,
                    Path.Combine(directory.Path, "Models"),
                    overwriteFiles: false);

                // Assert
                var expectedContextPath = Path.Combine(directory.Path, "Contexts", Constants.Files.CSharpFiles.DbContextFile);
                var expectedCategoryPath = Path.Combine(directory.Path, "Models", Constants.Files.CSharpFiles.CategoryFile);
                var expectedProductPath = Path.Combine(directory.Path, "Models", Constants.Files.CSharpFiles.ProductFile);
                Assert.Equal(expectedContextPath, result.ContextFile);
                Assert.False(File.Exists(expectedCategoryPath));
                Assert.False(File.Exists(expectedProductPath));
            }
        }

        [Fact]
        public void Save_Should_Write_Entity_Files()
        {
            using (var directory = new TempDirectory())
            {
                // Arrange
                var scaffolder = CreateScaffolder(ReverseEngineerOptions.EntitiesOnly, false);
                var model = scaffolder.ScaffoldModel(
                    connectionString: Constants.Connections.SqlServerConnection,
                    databaseOptions: new DatabaseModelFactoryOptions(),
                    modelOptions: new ModelReverseEngineerOptions(),
                    codeOptions: new ModelCodeGenerationOptions
                    {
                        ContextNamespace = Constants.Parameters.RootNamespace,
                        ModelNamespace = Constants.Parameters.RootNamespace,
                        ContextName = Constants.Parameters.ContextName,
                        ContextDir = Path.Combine(directory.Path, "Contexts"),
                        UseDataAnnotations = false,
                        Language = "C#",
                    });

                // Act
                var result = scaffolder.Save(model,
                    Path.Combine(directory.Path, "Models"),
                    overwriteFiles: false);

                // Assert
                var expectedContextPath = Path.Combine(directory.Path, "Contexts", Constants.Files.CSharpFiles.DbContextFile);
                var expectedCategoryPath = Path.Combine(directory.Path, "Models", Constants.Files.CSharpFiles.CategoryFile);
                var expectedProductPath = Path.Combine(directory.Path, "Models", Constants.Files.CSharpFiles.ProductFile);
                Assert.Equal(expectedCategoryPath, result.AdditionalFiles[0]);
                Assert.Equal(expectedProductPath, result.AdditionalFiles[1]);
                Assert.False(File.Exists(expectedContextPath));
            }
        }

        [Fact]
        public void Save_Should_Write_Context_and_Entity_Files()
        {
            using (var directory = new TempDirectory())
            {
                // Arrange
                var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextAndEntities, false);
                var model = scaffolder.ScaffoldModel(
                    connectionString: Constants.Connections.SqlServerConnection,
                    databaseOptions: new DatabaseModelFactoryOptions(),
                    modelOptions: new ModelReverseEngineerOptions(),
                    codeOptions: new ModelCodeGenerationOptions
                    {
                        ContextNamespace = Constants.Parameters.RootNamespace,
                        ModelNamespace = Constants.Parameters.RootNamespace,
                        ContextName = Constants.Parameters.ContextName,
                        ContextDir = Path.Combine(directory.Path, "Contexts"),
                        UseDataAnnotations = false,
                        Language = "C#",
                    });

                // Act
                var result = scaffolder.Save(model,
                    Path.Combine(directory.Path, "Models"),
                    overwriteFiles: false);

                // Assert
                var expectedContextPath = Path.Combine(directory.Path, "Contexts", Constants.Files.CSharpFiles.DbContextFile);
                var expectedCategoryPath = Path.Combine(directory.Path, "Models", Constants.Files.CSharpFiles.CategoryFile);
                var expectedProductPath = Path.Combine(directory.Path, "Models", Constants.Files.CSharpFiles.ProductFile);
                Assert.Equal(expectedContextPath, result.ContextFile);
                Assert.Equal(expectedCategoryPath, result.AdditionalFiles[0]);
                Assert.Equal(expectedProductPath, result.AdditionalFiles[1]);
            }
        }

        private IReverseEngineerScaffolder CreateScaffolder(ReverseEngineerOptions options, bool usePluralizer)
        {
            var fileService = new InMemoryTemplateFileService();
            fileService.InputFiles(ContextClassTemplate, ContextImportsTemplate, ContextCtorTemplate, ContextDbSetsTemplate,
                EntityClassTemplate, EntityImportsTemplate, EntityCtorTemplate, EntityPropertiesTemplate);

            var services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<IDbContextTemplateService, HbsDbContextTemplateService>()
                .AddSingleton<IEntityTypeTemplateService, HbsEntityTypeTemplateService>()
                .AddSingleton<IEntityTypeTransformationService, HbsEntityTypeTransformationService>()
                .AddSingleton<ITemplateFileService>(fileService)
                .AddSingleton<ITemplateLanguageService, CSharpTemplateLanguageService>()
                .AddSingleton<IModelCodeGenerator, HbsCSharpModelGenerator>()
                .AddSingleton(provider =>
                {
                    ICSharpDbContextGenerator contextGenerator = new HbsCSharpDbContextGenerator(
                        provider.GetRequiredService<IProviderConfigurationCodeGenerator>(),
                        provider.GetRequiredService<IAnnotationCodeGenerator>(),
                        provider.GetRequiredService<IDbContextTemplateService>(),
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
                        provider.GetRequiredService<ICSharpHelper>(),
                        provider.GetRequiredService<IOptions<HandlebarsScaffoldingOptions>>());
                    return options == ReverseEngineerOptions.DbContextOnly ||
                           options == ReverseEngineerOptions.DbContextAndEntities
                        ? contextGenerator
                        : new NullCSharpDbContextGenerator();
                })
                .AddSingleton(provider =>
                {
                    ICSharpEntityTypeGenerator entityGenerator = new HbsCSharpEntityTypeGenerator(
                        provider.GetRequiredService<ICSharpHelper>(),
                        provider.GetRequiredService<IEntityTypeTemplateService>(),
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
                        provider.GetRequiredService<IOptions<HandlebarsScaffoldingOptions>>());
                    return options == ReverseEngineerOptions.EntitiesOnly ||
                           options == ReverseEngineerOptions.DbContextAndEntities
                        ? entityGenerator
                        : new NullCSharpEntityTypeGenerator();
                })
                .AddSingleton<IHbsHelperService>(provider =>
                new HbsHelperService(new Dictionary<string, Action<TextWriter, Dictionary<string, object>, object[]>>
                {
                    {EntityFrameworkCore.Scaffolding.Handlebars.Helpers.Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                }))
                .AddSingleton<IHbsBlockHelperService>(provider =>
                new HbsBlockHelperService(new Dictionary<string, Action<TextWriter, HelperOptions, Dictionary<string, object>, object[]>>()))
                .AddSingleton<IReverseEngineerScaffolder, HbsReverseEngineerScaffolder>();

            if (usePluralizer)
                services.AddSingleton<IPluralizer, HumanizerPluralizer>();

            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
            var scaffolder = services
                .BuildServiceProvider()
                .GetRequiredService<IReverseEngineerScaffolder>();
            return scaffolder;
        }

        private Dictionary<string, string> GetGeneratedFiles(ScaffoldedModel model, ReverseEngineerOptions options)
        {
            var generatedFiles = new Dictionary<string, string>();

            if (options == ReverseEngineerOptions.DbContextOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                generatedFiles.Add(Constants.Files.CSharpFiles.DbContextFile, model.ContextFile.Code);
            }

            if (options == ReverseEngineerOptions.EntitiesOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                generatedFiles.Add(Constants.Files.CSharpFiles.CategoryFile, model.AdditionalFiles[0].Code);
                generatedFiles.Add(Constants.Files.CSharpFiles.ProductFile, model.AdditionalFiles[1].Code);
            }

            return generatedFiles;
        }
    }
}