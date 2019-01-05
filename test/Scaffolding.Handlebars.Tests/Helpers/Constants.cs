namespace Scaffolding.Handlebars.Tests.Helpers
{
    public static class Constants
    {
        public static class Connections
        {
            public const string SqLiteConnection = "DataSource=:memory:";
            public const string SqlServerConnection = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindTestDb; Integrated Security=True";
        }

        public static class Parameters
        {
            public const string ProjectPath = "FakeProjectPath";
            public const string RootNamespace = "FakeNamespace";
            public const string ContextName = "FakeDbContext";
        }

        public static class Templates
        {
            public const string ProjectFolder = "EntityFrameworkCore.Scaffolding.Handlebars";
            public const string CodeTemplatesFolder = "CodeTemplates";
            public const string CodeTemplatesAltFolder = "CodeTemplatesAlt";
            public const string EntityTypeFolder = "CSharpEntityType";
            public const string ContextFolder = "CSharpDbContext";
            public const string PartialsFolder = "Partials";
            public const string EntityClassFile = "Class.hbs";
            public const string EntityImportsFile = "Imports.hbs";
            public const string EntityCtorFile = "Constructor.hbs";
            public const string EntityPropertiesFile = "Properties.hbs";
            public const string ContextClassFile = "DbContext.hbs";
            public const string ContextImportsFile = "DbImports.hbs";
            public const string ContextCtorFile = "DbConstructor.hbs";
            public const string ContextDbSetsFile = "DbSets.hbs";
        }

        public static class Files
        {
            public const string DbContextFile = Parameters.ContextName + ".cs";
            public const string CategoryFile = "Category.cs";
            public const string ProductFile = "Product.cs";
        }
    }
}