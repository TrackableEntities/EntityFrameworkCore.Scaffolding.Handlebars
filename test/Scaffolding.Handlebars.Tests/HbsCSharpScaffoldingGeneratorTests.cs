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
        private InputFile ClassTemplate { get; }
        private InputFile ImportsTemplate { get; }
        private InputFile CtorTemplate { get; }
        private InputFile PropertiesTemplate { get; }

        public HbsCSharpScaffoldingGeneratorTests(NorthwindDbContextFixture fixture)
        {
            Fixture = fixture;
            Fixture.Initialize(useInMemory: false);

            var templatesVirtualPath =
                $"{Constants.Templates.CodeTemplatesFolder}/{Constants.Templates.EntityTypeFolder}";
            var partialsVirtualPath = templatesVirtualPath + $"/{Constants.Templates.PartialsFolder}";
            var projectRootDir = Path.Combine("..", "..", "..", "..", "..");
            var templatesPath = Path.Combine(projectRootDir, "src", Constants.Templates.ProjectFolder, 
                Constants.Templates.CodeTemplatesFolder, Constants.Templates.EntityTypeFolder);
            var partialTemplatesPath = Path.Combine(templatesPath, Constants.Templates.PartialsFolder);

            ClassTemplate = new InputFile
            {
                Directory = templatesVirtualPath,
                File = Constants.Templates.ClassFile,
                Contents = File.ReadAllText(Path.Combine(templatesPath, Constants.Templates.ClassFile))
            };
            ImportsTemplate = new InputFile
            {
                Directory = partialsVirtualPath,
                File = Constants.Templates.ImportsFile,
                Contents = File.ReadAllText(Path.Combine(partialTemplatesPath, Constants.Templates.ImportsFile))
            };
            CtorTemplate = new InputFile
            {
                Directory = partialsVirtualPath,
                File = Constants.Templates.CtorFile,
                Contents = File.ReadAllText(Path.Combine(partialTemplatesPath, Constants.Templates.CtorFile))
            };
            PropertiesTemplate = new InputFile
            {
                Directory = partialsVirtualPath,
                File = Constants.Templates.PropertiesFile,
                Contents = File.ReadAllText(Path.Combine(partialTemplatesPath, Constants.Templates.PropertiesFile))
            };
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WriteCode_Should_Generate_Entity_Files(bool useDataAnnotations)
        {
            // Arrange
            var fileService = new InMemoryTemplateFileService();
            fileService.InputFiles(ClassTemplate, ImportsTemplate, CtorTemplate, PropertiesTemplate);
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
                connectionString: Constants.Connections.SqlServerConnection,
                tables: Enumerable.Empty<string>(),
                schemas: Enumerable.Empty<string>(),
                projectPath: "FakeProjectPath",
                outputPath: null,
                rootNamespace: "FakeNamespace",
                contextName: "NorthwindDbContext",
                useDataAnnotations: useDataAnnotations,
                overwriteFiles: false,
                useDatabaseNames: false);

            // Assert
            var categoryPath = files.EntityTypeFiles[0];
            var category = fileService.RetrieveFileContents(
                Path.GetDirectoryName(categoryPath), Path.GetFileName(categoryPath));
            var productPath = files.EntityTypeFiles[1];
            var product = fileService.RetrieveFileContents(
                Path.GetDirectoryName(productPath), Path.GetFileName(productPath));

            object expectedCategory;
            object expectedProduct;

            if (useDataAnnotations)
            {
                expectedCategory = ExpectedWithAnnotations.CategoryClass;
                expectedProduct = ExpectedWithAnnotations.ProductClass;
            }
            else
            {
                expectedCategory = Expected.CategoryClass;
                expectedProduct = Expected.ProductClass;
            }

            Assert.Equal(expectedCategory, category);
            Assert.Equal(expectedProduct, product);
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
        }

        private static class ExpectedWithAnnotations
        {
            public const string CategoryClass =
@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeNamespace
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; }

        [InverseProperty(""Category"")]
        public ICollection<Product> Product { get; set; }
    }
}
";

            public const string ProductClass =
@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeNamespace
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public bool Discontinued { get; set; }
        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }
        public byte[] RowVersion { get; set; }
        [Column(TypeName = ""money"")]
        public decimal? UnitPrice { get; set; }

        [ForeignKey(""CategoryId"")]
        [InverseProperty(""Product"")]
        public Category Category { get; set; }
    }
}
";
        }
    }
}