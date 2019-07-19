using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
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
        [InlineData(false)]
        [InlineData(true)]
        public void WriteCode_Should_Generate_Context_File(bool useDataAnnotations)
        {
            // Arrange
            var options = ReverseEngineerOptions.DbContextOnly;
            var scaffolder = CreateScaffolder(options);

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

            // Act
            var files = GetGeneratedFiles(model, options);

            // Assert
            object expectedContext = useDataAnnotations
                ? ExpectedContextsWithAnnotations.ContextClass
                : ExpectedContexts.ContextClass;

            var context = files[Constants.Files.CSharpFiles.DbContextFile];

            Assert.Equal(expectedContext, context);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WriteCode_Should_Generate_Entity_Files(bool useDataAnnotations)
        {
            // Arrange
            var options = ReverseEngineerOptions.EntitiesOnly;
            var scaffolder = CreateScaffolder(options);

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

            // Act
            var files = GetGeneratedFiles(model, options);

            // Assert
            var category = files[Constants.Files.CSharpFiles.CategoryFile];
            var product = files[Constants.Files.CSharpFiles.ProductFile];

            object expectedCategory = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.CategoryClass
                : ExpectedEntities.CategoryClass;
            object expectedProduct = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.ProductClass
                : ExpectedEntities.ProductClass;

            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WriteCode_Should_Generate_Context_and_Entity_Files(bool useDataAnnotations)
        {
            // Arrange
            var options = ReverseEngineerOptions.DbContextAndEntities;
            var scaffolder = CreateScaffolder(options);

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

            // Act
            Dictionary<string, string> files = GetGeneratedFiles(model, options);

            // Assert
            object expectedContext = useDataAnnotations
                ? ExpectedContextsWithAnnotations.ContextClass
                : ExpectedContexts.ContextClass;
            object expectedCategory = useDataAnnotations
                ? ExpectedEntitiesWithAnnotations.CategoryClass
                : ExpectedEntities.CategoryClass;
            object expectedProduct = useDataAnnotations
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
            using (var directory = new TempDirectory())
            {
                // Arrange
                var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextOnly);
                var model = scaffolder.ScaffoldModel(
                    connectionString: Constants.Connections.SqlServerConnection,
                    tables: Enumerable.Empty<string>(),
                    schemas: Enumerable.Empty<string>(),
                    @namespace: Constants.Parameters.RootNamespace,
                    language: "C#",
                    contextName: Constants.Parameters.ContextName,
                    modelOptions: new ModelReverseEngineerOptions(),
                    contextDir: Path.Combine(directory.Path, "Contexts"),
                    codeOptions: new ModelCodeGenerationOptions { UseDataAnnotations = false });

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
                var scaffolder = CreateScaffolder(ReverseEngineerOptions.EntitiesOnly);
                var model = scaffolder.ScaffoldModel(
                    connectionString: Constants.Connections.SqlServerConnection,
                    tables: Enumerable.Empty<string>(),
                    schemas: Enumerable.Empty<string>(),
                    @namespace: Constants.Parameters.RootNamespace,
                    language: "C#",
                    contextName: Constants.Parameters.ContextName,
                    modelOptions: new ModelReverseEngineerOptions(),
                    contextDir: Path.Combine(directory.Path, "Contexts"),
                    codeOptions: new ModelCodeGenerationOptions { UseDataAnnotations = false });

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
                var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextAndEntities);
                var model = scaffolder.ScaffoldModel(
                    connectionString: Constants.Connections.SqlServerConnection,
                    tables: Enumerable.Empty<string>(),
                    schemas: Enumerable.Empty<string>(),
                    @namespace: Constants.Parameters.RootNamespace,
                    language: "C#",
                    contextName: Constants.Parameters.ContextName,
                    modelOptions: new ModelReverseEngineerOptions(),
                    contextDir: Path.Combine(directory.Path, "Contexts"),
                    codeOptions: new ModelCodeGenerationOptions { UseDataAnnotations = false });

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

        private IReverseEngineerScaffolder CreateScaffolder(ReverseEngineerOptions options)
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
#pragma warning disable 618
                        provider.GetRequiredService<IEnumerable<IScaffoldingProviderCodeGenerator>>(),
#pragma warning restore 618
                        provider.GetRequiredService<IEnumerable<IProviderConfigurationCodeGenerator>>(),
                        provider.GetRequiredService<IAnnotationCodeGenerator>(),
                        provider.GetRequiredService<IDbContextTemplateService>(),
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
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
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
                        provider.GetRequiredService<ICSharpHelper>());
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