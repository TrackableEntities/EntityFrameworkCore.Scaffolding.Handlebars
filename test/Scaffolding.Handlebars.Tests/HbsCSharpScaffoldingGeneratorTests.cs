using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Scaffolding.Handlebars.Tests.Fakes;
using Scaffolding.Handlebars.Tests.Helpers;
using Scaffolding.Handlebars.Tests.Internal;
using Xunit;

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
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.DbContextFolder}";
            var contextPartialsVirtualPath = contextTemplatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var contextTemplatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder,
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.DbContextFolder);
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
        //[InlineData(false)]
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
            var dbContextTemplateService = new HbsDbContextTemplateService(fileService);
            var entityTypeTemplateService = new HbsEntityTypeTemplateService(fileService);

            ICSharpUtilities cSharpUtilities = new CSharpUtilities();
            ICSharpDbContextGenerator realContextGenerator = new HbsCSharpDbContextGenerator(
                new SqlServerScaffoldingCodeGenerator(),
                new SqlServerAnnotationCodeGenerator(new AnnotationCodeGeneratorDependencies()),
                cSharpUtilities,
                new HbsDbContextTemplateService(fileService));
            ICSharpDbContextGenerator contextGenerator =
                options == ReverseEngineerOptions.DbContextOnly || options == ReverseEngineerOptions.DbContextOnly
                ? realContextGenerator
                : new NullCSharpDbContextGenerator();
            ICSharpEntityTypeGenerator realEntityGenerator = new HbsCSharpEntityTypeGenerator(
                cSharpUtilities,
                new HbsEntityTypeTemplateService(fileService));
            ICSharpEntityTypeGenerator entityGenerator =
                options == ReverseEngineerOptions.EntitiesOnly || options == ReverseEngineerOptions.DbContextAndEntities
                ? realEntityGenerator
                : new NullCSharpEntityTypeGenerator();
            IScaffoldingCodeGenerator scaffoldingGenerator = new HbsCSharpScaffoldingGenerator(
                fileService, dbContextTemplateService, entityTypeTemplateService, 
                contextGenerator, entityGenerator);

            var modelFactory = new SqlServerDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    new TestSqlLoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("Fake")));
            var reverseEngineer = new ReverseEngineerScaffolder(
                modelFactory,
                new FakeScaffoldingModelFactory(new TestOperationReporter()),
                scaffoldingGenerator, cSharpUtilities);

            // Act
            var files = reverseEngineer.Generate(
                connectionString: Constants.Connections.SqlServerConnection,
                tables: Enumerable.Empty<string>(),
                schemas: Enumerable.Empty<string>(),
                projectPath: Constants.Parameters.ProjectPath,
                outputPath: null,
                rootNamespace: Constants.Parameters.RootNamespace,
                contextName: Constants.Parameters.ContextName,
                useDataAnnotations: useDataAnnotations,
                overwriteFiles: false,
                useDatabaseNames: false);

            var generatedFiles = new Dictionary<string, string>();

            if (options == ReverseEngineerOptions.DbContextOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                var contextPath = files.ContextFile;
                var context = fileService.RetrieveFileContents(
                    Path.GetDirectoryName(contextPath), Path.GetFileName(contextPath));
                generatedFiles.Add(Constants.Files.DbContextFile, context);
            }

            if (options == ReverseEngineerOptions.EntitiesOnly
                || options == ReverseEngineerOptions.DbContextAndEntities)
            {
                var categoryPath = files.EntityTypeFiles[0];
                var category = fileService.RetrieveFileContents(
                    Path.GetDirectoryName(categoryPath), Path.GetFileName(categoryPath));
                var productPath = files.EntityTypeFiles[1];
                var product = fileService.RetrieveFileContents(
                    Path.GetDirectoryName(productPath), Path.GetFileName(productPath));
                generatedFiles.Add(Constants.Files.CategoryFile, category);
                generatedFiles.Add(Constants.Files.ProductFile, product);
            }

            return generatedFiles;
        }
    }
}