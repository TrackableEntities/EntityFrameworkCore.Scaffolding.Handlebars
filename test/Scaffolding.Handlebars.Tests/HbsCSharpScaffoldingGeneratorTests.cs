using System.Diagnostics;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Scaffolding.Handlebars.Tests.Fakes;
using Scaffolding.Handlebars.Tests.Helpers;
using Scaffolding.Handlebars.Tests.Internal;
using Xunit;

namespace Scaffolding.Handlebars.Tests
{
    [Collection("NorthwindDbContext")]
    public class HbsCSharpScaffoldingGeneratorTests
    {
        private NorthwindDbContextFixture Fixture { get; }

        public HbsCSharpScaffoldingGeneratorTests(NorthwindDbContextFixture fixture)
        {
            Fixture = fixture;
            Fixture.Initialize(useInMemory: false);
        }

        [Fact]
        public void WriteCode_Should_Generate_Entity_Files()
        {
            // Arrange
            var inputTemplate = (
                directoryName: "CodeTemplates/CSharpEntityType/Partials",
                fileName: "Import.hbs",
                contents: "using {{import}};");
            var propertyTemplate = (
                directoryName: "CodeTemplates/CSharpEntityType/Partials",
                fileName: "Property.hbs",
                contents: "public {{type}} {{name}} { get; set; }");

            var fileService = new InMemoryTemplateFileService();
            fileService.InputFiles(inputTemplate, propertyTemplate);
            var templateService = new HbsEntityTypeTemplateService(fileService);

            var cSharpUtilities = new CSharpUtilities();
            var entityGenerator = new HbsCSharpEntityTypeGenerator(
                cSharpUtilities, new HbsEntityTypeTemplateService(fileService));
            var scaffoldingGenerator = new HbsCSharpScaffoldingGenerator(
                fileService, templateService, new FakeCSharpDbContextGenerator(), entityGenerator);

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
                connectionString: Constants.SqlServerConnection,
                tables: Enumerable.Empty<string>(),
                schemas: Enumerable.Empty<string>(),
                projectPath: "FakeProjectPath",
                outputPath: null,
                rootNamespace: "FakeNamespace",
                contextName: "NorthwindDbContext",
                useDataAnnotations: false,
                overwriteFiles: false,
                useDatabaseNames: false);

            // Assert
            var categoryPath = files.EntityTypeFiles[0];
            var categoryContents = fileService.RetrieveFileContents(
                Path.GetDirectoryName(categoryPath), Path.GetFileName(categoryPath));
            var productPath = files.EntityTypeFiles[1];
            var productContents = fileService.RetrieveFileContents(
                Path.GetDirectoryName(productPath), Path.GetFileName(productPath));

            Assert.Equal(Expected.CategoryClass, categoryContents);
            Assert.Equal(Expected.ProductClass, productContents);
        }

        private static class Expected
        {
            public const string CategoryClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
";

            public const string ProductClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public bool Discontinued { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }

        public Category Category { get; set; }
    }
}
";
        }
    }
}