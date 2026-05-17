namespace Scaffolding.Handlebars.Tests.Helpers
{
    public static class Constants
    {
        public static class Connections
        {
            public const string NorthwindTest = "NorthwindTestContext";
        }

        public static class CollectionDefinitions
        {
            public const string DatabaseCollection = "Database Collection";
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
            public const string PartialsFolder = "Partials";
            public const string EntityClassFile = "Class.hbs";
            public const string EntityImportsFile = "Imports.hbs";
            public const string EntityCtorFile = "Constructor.hbs";
            public const string EntityPropertiesFile = "Properties.hbs";
            public const string ContextClassFile = "DbContext.hbs";
            public const string ContextImportsFile = "DbImports.hbs";
            public const string ContextCtorFile = "DbConstructor.hbs";
            public const string ContextOnConfiguringFile = "DbOnConfiguring.hbs";
            public const string ContextDbSetsFile = "DbSets.hbs";
            public static class CSharpTemplateDirectories
            {
                public const string EntityTypeFolder = "CSharpEntityType";
                public const string ContextFolder = "CSharpDbContext";
            }
            public static class TypeScriptTemplateDirectories
            {
                public const string EntityTypeFolder = "TypeScriptEntityType";
                public const string ContextFolder = "TypeScriptDbContext";
            }
        }

        public static class Names
        {
            public const string Category = "Category";
            public const string Customer = "Customer";
            public const string Product = "Product";

            public static class Transformed
            {
                public const string Category = Names.Category + "Renamed";
                public const string Customer = Names.Customer + "Renamed";
                public const string Product = Names.Product + "Renamed";
            }

            public static class Transformed2
            {
                public const string Category = "dbo_" + Transformed.Category;
                public const string Customer = "dbo_" + Transformed.Customer;
                public const string Product = "dbo_" + Transformed.Product;
            }
        }

        public static class Files
        {
            public static class CSharpFiles
            {
                public const string DbContextFile = Parameters.ContextName + ".cs";
                public const string CategoryFile = "Category.cs";
                public const string CategoryFileTransformed2 = "dbo_" + CategoryFile;
                public const string CustomerFile = "Customer.cs";
                public const string CustomerFileTransformed2 = "dbo_" + CustomerFile;
                public const string ProductFile = "Product.cs";
                public const string ProductFileTransformed2 = "dbo_" + ProductFile;
            }
            public static class TypeScriptFiles
            {
                public const string DbContextFile = Parameters.ContextName + ".cs";
                public const string CategoryFile = "Category.ts";
                public const string CustomerFile = "Customer.ts";
                public const string ProductFile = "Product.ts";
            }
        }
    }
}