using System;
using System.Collections.Generic;
using System.IO;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scaffolding.Handlebars.Tests.Fakes;
using Scaffolding.Handlebars.Tests.Helpers;
using Xunit;
using Constants = Scaffolding.Handlebars.Tests.Helpers.Constants;

namespace Scaffolding.Handlebars.Tests
{
    [Collection(Constants.CollectionDefinitions.DatabaseCollection)]
    public partial class HbsTypeScriptScaffoldingGeneratorTests
    {
        private static string ConnectionString { get; set; }
        private InputFile ContextClassTemplate { get; }
        private InputFile ContextImportsTemplate { get; }
        private InputFile ContextCtorTemplate { get; }
        private InputFile ContextOnConfiguringTemplate { get; }
        private InputFile ContextDbSetsTemplate { get; }
        private InputFile EntityClassTemplate { get; }
        private InputFile EntityImportsTemplate { get; }
        private InputFile EntityCtorTemplate { get; }
        private InputFile EntityPropertiesTemplate { get; }

        public HbsTypeScriptScaffoldingGeneratorTests(DatabaseFixture fixture)
        {
            fixture.Initialize();
            ConnectionString = fixture.ConnectionString;

            var projectRootDir = Path.Combine("..", "..", "..", "..", "..");
            var contextTemplatesVirtualPath =
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.TypeScriptTemplateDirectories.ContextFolder}";
            var contextPartialsVirtualPath = contextTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var contextTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder,
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.TypeScriptTemplateDirectories.ContextFolder);
            var contextPartialTemplatesPath = Path.Combine(contextTemplatesPath, Constants.Templates.PartialsFolder);

            var entityTemplatesVirtualPath =
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.TypeScriptTemplateDirectories.EntityTypeFolder}";
            var entityPartialsVirtualPath = entityTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var entityTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder,
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.TypeScriptTemplateDirectories.EntityTypeFolder);
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

        [Fact]
        public void WriteCode_Should_Generate_Context_File()
        {
            // Arrange
            var options = ReverseEngineerOptions.DbContextOnly;
            var scaffolder = CreateScaffolder(options);

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: ConnectionString,
                databaseOptions: new DatabaseModelFactoryOptions(),
                modelOptions: new ModelReverseEngineerOptions(),
                codeOptions: new ModelCodeGenerationOptions
                {
                    ContextNamespace = Constants.Parameters.RootNamespace,
                    ModelNamespace = Constants.Parameters.RootNamespace,
                    ContextName = Constants.Parameters.ContextName,
                    ContextDir = Constants.Parameters.ProjectPath,
                    UseDataAnnotations = false,
                    Language = "C#",
                });

            // Assert
            var files = GetGeneratedFiles(model, options);
            object expectedContext = ExpectedContexts.ContextClass;

            var context = files[Constants.Files.TypeScriptFiles.DbContextFile];

            Assert.Equal(expectedContext, context);
        }

        [Fact]
        public void WriteCode_Should_Generate_Entity_Files()
        {
            // Arrange
            var options = ReverseEngineerOptions.EntitiesOnly;
            var scaffolder = CreateScaffolder(options);

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: ConnectionString,
                databaseOptions: new DatabaseModelFactoryOptions(),
                modelOptions: new ModelReverseEngineerOptions(),
                codeOptions: new ModelCodeGenerationOptions
                {
                    ContextNamespace = Constants.Parameters.RootNamespace,
                    ModelNamespace = Constants.Parameters.RootNamespace,
                    ContextName = Constants.Parameters.ContextName,
                    ContextDir = Constants.Parameters.ProjectPath,
                    UseDataAnnotations = false,
                    Language = "C#",
                });

            // Assert
            var files = GetGeneratedFiles(model, options);
            var category = files[Constants.Files.TypeScriptFiles.CategoryFile];
            var product = files[Constants.Files.TypeScriptFiles.ProductFile];

            object expectedCategory = ExpectedEntities.CategoryClass;
            object expectedProduct = ExpectedEntities.ProductClass;

            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
        }

        [Fact]
        public void WriteCode_Should_Generate_Context_and_Entity_Files()
        {
            // Arrange
            var options = ReverseEngineerOptions.DbContextAndEntities;
            var scaffolder = CreateScaffolder(options);

            // Act
            var model = scaffolder.ScaffoldModel(
                connectionString: ConnectionString,
                databaseOptions: new DatabaseModelFactoryOptions(),
                modelOptions: new ModelReverseEngineerOptions(),
                codeOptions: new ModelCodeGenerationOptions
                {
                    ContextNamespace = Constants.Parameters.RootNamespace,
                    ModelNamespace = Constants.Parameters.RootNamespace,
                    ContextName = Constants.Parameters.ContextName,
                    ContextDir = Constants.Parameters.ProjectPath,
                    UseDataAnnotations = false,
                    Language = "C#",
                });

            // Act
            Dictionary<string, string> files = GetGeneratedFiles(model, options);

            // Assert
            object expectedContext = ExpectedContexts.ContextClass;
            object expectedCategory = ExpectedEntities.CategoryClass;
            object expectedProduct = ExpectedEntities.ProductClass;

            var context = files[Constants.Files.TypeScriptFiles.DbContextFile];
            var category = files[Constants.Files.TypeScriptFiles.CategoryFile];
            var product = files[Constants.Files.TypeScriptFiles.ProductFile];

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
                connectionString: ConnectionString,
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
            var expectedContextPath = Path.Combine(directory.Path, "Contexts", Constants.Files.TypeScriptFiles.DbContextFile);
            var expectedCategoryPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.CategoryFile);
            var expectedProductPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.ProductFile);
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
                connectionString: ConnectionString,
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
            var expectedContextPath = Path.Combine(directory.Path, "Contexts", Constants.Files.TypeScriptFiles.DbContextFile);
            var expectedCategoryPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.CategoryFile);
            var expectedCustomerPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.CustomerFile);
            var expectedProductPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.ProductFile);
            Assert.Equal(expectedCategoryPath, result.AdditionalFiles[0]);
            Assert.Equal(expectedCustomerPath, result.AdditionalFiles[1]);
            Assert.Equal(expectedProductPath, result.AdditionalFiles[2]);
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
                connectionString: ConnectionString,
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
            var expectedContextPath = Path.Combine(directory.Path, "Contexts", $"{filenamePrefix}{Constants.Files.TypeScriptFiles.DbContextFile}");
            var expectedCategoryPath = Path.Combine(directory.Path, "Models", $"{filenamePrefix}{Constants.Files.TypeScriptFiles.CategoryFile}");
            var expectedCustomerPath = Path.Combine(directory.Path, "Models", $"{filenamePrefix}{Constants.Files.TypeScriptFiles.CustomerFile}");
            var expectedProductPath = Path.Combine(directory.Path, "Models", $"{filenamePrefix}{ Constants.Files.TypeScriptFiles.ProductFile}");
            Assert.Equal(expectedContextPath, result.ContextFile);
            Assert.Equal(expectedCategoryPath, result.AdditionalFiles[0]);
            Assert.Equal(expectedCustomerPath, result.AdditionalFiles[1]);
            Assert.Equal(expectedProductPath, result.AdditionalFiles[2]);
        }

        [Fact]
        public void Save_Should_Write_Context_and_Entity_Files()
        {
            using var directory = new TempDirectory();
            // Arrange
            var scaffolder = CreateScaffolder(ReverseEngineerOptions.DbContextAndEntities);
            var model = scaffolder.ScaffoldModel(
                connectionString: ConnectionString,
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
            var expectedContextPath = Path.Combine(directory.Path, "Contexts", Constants.Files.TypeScriptFiles.DbContextFile);
            var expectedCategoryPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.CategoryFile);
            var expectedCustomerPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.CustomerFile);
            var expectedProductPath = Path.Combine(directory.Path, "Models", Constants.Files.TypeScriptFiles.ProductFile);
            Assert.Equal(expectedContextPath, result.ContextFile);
            Assert.Equal(expectedCategoryPath, result.AdditionalFiles[0]);
            Assert.Equal(expectedCustomerPath, result.AdditionalFiles[1]);
            Assert.Equal(expectedProductPath, result.AdditionalFiles[2]);
        }

        private IReverseEngineerScaffolder CreateScaffolder(ReverseEngineerOptions options, string filenamePrefix = null)
        {
            var fileService = new InMemoryTemplateFileService();
            fileService.InputFiles(ContextClassTemplate, ContextImportsTemplate,
                ContextCtorTemplate, ContextOnConfiguringTemplate, ContextDbSetsTemplate,
                EntityClassTemplate, EntityImportsTemplate, EntityCtorTemplate, EntityPropertiesTemplate);

            var services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<IDbContextTemplateService, FakeHbsDbContextTemplateService>()
                .AddSingleton<IEntityTypeTemplateService, FakeHbsEntityTypeTemplateService>()
                .AddSingleton<ITypeScriptHelper, TypeScriptHelper>()
                .AddSingleton<ITemplateFileService>(fileService)
                .AddSingleton<ITemplateLanguageService, FakeTypeScriptTemplateLanguageService>()
                .AddSingleton<IModelCodeGenerator, HbsTypeScriptModelGenerator>()
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
                    ICSharpEntityTypeGenerator entityGenerator = new HbsTypeScriptEntityTypeGenerator(
                        provider.GetRequiredService<IAnnotationCodeGenerator>(),
                        provider.GetRequiredService<IEntityTypeTemplateService>(),
                        provider.GetRequiredService<IEntityTypeTransformationService>(),
                        provider.GetRequiredService<ICSharpHelper>(),
                        provider.GetRequiredService<ITypeScriptHelper>(),
                        provider.GetRequiredService<IOptions<HandlebarsScaffoldingOptions>>());
                    return options == ReverseEngineerOptions.EntitiesOnly ||
                           options == ReverseEngineerOptions.DbContextAndEntities
                        ? entityGenerator
                        : new NullCSharpEntityTypeGenerator();
                })
                .AddSingleton<IHbsHelperService>(provider =>
                    new HbsHelperService(new Dictionary<string, Action<EncodedTextWriter, Context, Arguments>>
                    {
                        {EntityFrameworkCore.Scaffolding.Handlebars.Helpers.Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                    }))
                .AddSingleton<IHbsBlockHelperService>(provider =>
                    new HbsBlockHelperService(new Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>>()))
                .AddSingleton<IReverseEngineerScaffolder, HbsReverseEngineerScaffolder>()
                .Configure<IOptions<HandlebarsScaffoldingOptions>>(_ => new HandlebarsScaffoldingOptions());


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
                generatedFiles.Add(Constants.Files.TypeScriptFiles.DbContextFile, model.ContextFile.Code);
            }

            if (options == ReverseEngineerOptions.EntitiesOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                generatedFiles.Add(Constants.Files.TypeScriptFiles.CategoryFile, model.AdditionalFiles[0].Code);
                generatedFiles.Add(Constants.Files.TypeScriptFiles.CustomerFile, model.AdditionalFiles[1].Code);
                generatedFiles.Add(Constants.Files.TypeScriptFiles.ProductFile, model.AdditionalFiles[2].Code);
            }

            return generatedFiles;
        }
    }
}