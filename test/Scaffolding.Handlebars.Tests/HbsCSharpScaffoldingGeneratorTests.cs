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
using Scaffolding.Handlebars.Tests.Fakes;
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
        private InputFile ContextOnConfiguringTemplate { get; }
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
            ContextOnConfiguringTemplate = new InputFile
            {
                Directory = contextPartialsVirtualPath,
                File = Constants.Templates.ContextOnConfiguringFile,
                Contents = File.ReadAllText(Path.Combine(contextPartialTemplatesPath, Constants.Templates.ContextOnConfiguringFile))
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
        [InlineData(false, "en-US", true)]
        [InlineData(false, "en-US", false)]
        [InlineData(true, "en-US", false)]
        [InlineData(false, "tr-TR", false)]
        [InlineData(true, "tr-TR", false)]
        public void WriteCode_Should_Generate_Context_File(bool useDataAnnotations, string culture, bool suppressOnConfiguring)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var options = ReverseEngineerOptions.DbContextOnly;
            var scaffolder = CreateScaffolder(options);

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
                    SuppressOnConfiguring = suppressOnConfiguring
                });

            // Assert
            var files = GetGeneratedFiles(model, options);
            object expectedContext;
            expectedContext = useDataAnnotations
                ? ExpectedContextsWithAnnotations.ContextClass
                : ExpectedContexts.ContextClass;
            if (suppressOnConfiguring)
            {
                expectedContext = ExpectedContextsSupressOnConfiguring.ContextClass;
            }
            var context = files[Constants.Files.CSharpFiles.DbContextFile];

            Assert.Equal(expectedContext, context);
        }

        [Theory]
        [InlineData(false, "en-US")]
        [InlineData(true, "en-US")]
        [InlineData(false, "tr-TR")]
        [InlineData(true, "tr-TR")]
        public void WriteCode_Should_Generate_Entity_Files(bool useDataAnnotations, string culture)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var options = ReverseEngineerOptions.EntitiesOnly;
            var scaffolder = CreateScaffolder(options);

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
            expectedCategory = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.CategoryClass
                : ExpectedEntities.CategoryClass;
            expectedProduct = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.ProductClass
                : ExpectedEntities.ProductClass;
            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("tr-TR")]
        public void WriteCode_Should_Generate_Entities_With_Nullable_Navigation_When_Configured(string culture)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var revEngOptions = ReverseEngineerOptions.EntitiesOnly;
            var scaffolder = CreateScaffolder(revEngOptions);

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
                    UseDataAnnotations = false,
                    UseNullableReferenceTypes = true,
                    Language = "C#",
                });

            // Assert
            var files = GetGeneratedFiles(model, revEngOptions);
            var category = files[Constants.Files.CSharpFiles.CategoryFile];
            var product = files[Constants.Files.CSharpFiles.ProductFile];

            object expectedCategory;
            object expectedProduct;
            expectedCategory = ExpectedEntitiesWithNullableNavigation.CategoryClass;
            expectedProduct = ExpectedEntitiesWithNullableNavigation.ProductClass;
            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        [Theory]
        [InlineData(false, "en-US")]
        [InlineData(true, "en-US")]
        [InlineData(false, "tr-TR")]
        [InlineData(true, "tr-TR")]
        public void WriteCode_Should_Generate_Context_and_Entity_Files(bool useDataAnnotations, string culture)
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            var options = ReverseEngineerOptions.DbContextAndEntities;
            var scaffolder = CreateScaffolder(options);

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
            expectedContext = useDataAnnotations
                ? ExpectedContextsWithAnnotations.ContextClass
                : ExpectedContexts.ContextClass;
            expectedCategory = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.CategoryClass
                : ExpectedEntities.CategoryClass;
            expectedProduct = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.ProductClass
                : ExpectedEntities.ProductClass;
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
            using var directory = new TempDirectory();
            // Arrange
            var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextOnly);
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

        [Fact]
        public void Save_Should_Write_Entity_Files()
        {
            using var directory = new TempDirectory();
            // Arrange
            var scaffolder = CreateScaffolder(ReverseEngineerOptions.EntitiesOnly);
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

        [Fact]
        public void Save_Should_Write_Context_and_Entity_Files_With_Prefix()
        {
            using var directory = new TempDirectory();
            // Arrange
            var filenamePrefix = "prefix.";
            var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextAndEntities, filenamePrefix);
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
            var expectedContextPath = Path.Combine(directory.Path, "Contexts", $"{filenamePrefix}{Constants.Files.CSharpFiles.DbContextFile}");
            var expectedCategoryPath = Path.Combine(directory.Path, "Models", $"{filenamePrefix}{Constants.Files.CSharpFiles.CategoryFile}");
            var expectedProductPath = Path.Combine(directory.Path, "Models", $"{filenamePrefix}{ Constants.Files.CSharpFiles.ProductFile}");
            Assert.Equal(expectedContextPath, result.ContextFile);
            Assert.Equal(expectedCategoryPath, result.AdditionalFiles[0]);
            Assert.Equal(expectedProductPath, result.AdditionalFiles[1]);
        }

        [Fact]
        public void Save_Should_Write_Context_and_Entity_Files()
        {
            using var directory = new TempDirectory();
            // Arrange
            var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextAndEntities);
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

        private IReverseEngineerScaffolder CreateScaffolder(ReverseEngineerOptions revEngOptions, string filenamePrefix = null)
        {
            return CreateScaffolder(revEngOptions, _ => { }, filenamePrefix);
        }

        private IReverseEngineerScaffolder CreateScaffolder(ReverseEngineerOptions revEngOptions, Action<HandlebarsScaffoldingOptions> configureOptions, string filenamePrefix = null)
        {
            var fileService = new InMemoryTemplateFileService();
            fileService.InputFiles(ContextClassTemplate, ContextImportsTemplate,
                ContextCtorTemplate, ContextOnConfiguringTemplate, ContextDbSetsTemplate,
                EntityClassTemplate, EntityImportsTemplate, EntityCtorTemplate, EntityPropertiesTemplate);

            var services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<IDbContextTemplateService, FakeHbsDbContextTemplateService>()
                .AddSingleton<IEntityTypeTemplateService, FakeHbsEntityTypeTemplateService>()
                .AddSingleton<ITemplateFileService>(fileService)
                .AddSingleton<ITemplateLanguageService, FakeCSharpTemplateLanguageService>()
                .AddSingleton<IModelCodeGenerator, HbsCSharpModelGenerator>();

#pragma warning disable EF1001 // Internal EF Core API usage.
            services
                .AddSingleton(provider =>
                {
                    ICSharpDbContextGenerator contextGenerator = new HbsCSharpDbContextGenerator(
                        provider.GetRequiredService<IProviderConfigurationCodeGenerator>(),
                        provider.GetRequiredService<IAnnotationCodeGenerator>(),
                        provider.GetRequiredService<IDbContextTemplateService>(),
                        provider.GetRequiredService<IContextTransformationService>(),
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
                        provider.GetRequiredService<ICSharpHelper>(),
                        provider.GetRequiredService<IOptions<HandlebarsScaffoldingOptions>>());
                    return revEngOptions == ReverseEngineerOptions.DbContextOnly ||
                           revEngOptions == ReverseEngineerOptions.DbContextAndEntities
                        ? contextGenerator
                        : new NullCSharpDbContextGenerator();
                })
                .AddSingleton(provider =>
                {
                    ICSharpEntityTypeGenerator entityGenerator = new HbsCSharpEntityTypeGenerator(
                        provider.GetRequiredService<IAnnotationCodeGenerator>(),
                        provider.GetRequiredService<ICSharpHelper>(),
                        provider.GetRequiredService<IEntityTypeTemplateService>(),
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
                        provider.GetRequiredService<IOptions<HandlebarsScaffoldingOptions>>());
                    return revEngOptions == ReverseEngineerOptions.EntitiesOnly ||
                           revEngOptions == ReverseEngineerOptions.DbContextAndEntities
                        ? entityGenerator
                        : new NullCSharpEntityTypeGenerator();
                });
#pragma warning restore EF1001 // Internal EF Core API usage.

            services
                .AddSingleton<IHbsHelperService>(provider =>
                new HbsHelperService(new Dictionary<string, Action<EncodedTextWriter, Context, Arguments>>
                {
                    {EntityFrameworkCore.Scaffolding.Handlebars.Helpers.Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                }))
                .AddSingleton<IHbsBlockHelperService>(provider =>
                new HbsBlockHelperService(new Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>>()))
                .AddSingleton<IReverseEngineerScaffolder, HbsReverseEngineerScaffolder>();


            if (string.IsNullOrWhiteSpace(filenamePrefix))
            {
                services
                    .AddSingleton<IContextTransformationService, HbsContextTransformationService>()
                    .AddSingleton<IEntityTypeTransformationService, HbsEntityTypeTransformationService>();
            }
            else
            {
                services
                    .AddSingleton<IContextTransformationService>(y => new HbsContextTransformationService(contextName => $"{filenamePrefix}{contextName}"))
                    .AddSingleton<IEntityTypeTransformationService>(y => new HbsEntityTypeTransformationService(entityFileNameTransformer: entityName => $"{filenamePrefix}{entityName}"));
            }

            services.Configure(configureOptions);

#pragma warning disable EF1001 // Internal EF Core API usage.
            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
#pragma warning restore EF1001 // Internal EF Core API usage.
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