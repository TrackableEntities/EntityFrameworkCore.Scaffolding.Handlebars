namespace Scaffolding.Handlebars.Tests.Helpers
{
    public static class Constants
    {
        public static class Connections
        {
            public const string SqLiteConnection = "DataSource=:memory:";
            public const string SqlServerConnection = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindTestDb; Integrated Security=True; MultipleActiveResultSets=True";
        }

        public static class Templates
        {
            public const string ProjectFolder = "EntityFrameworkCore.Scaffolding.Handlebars";
            public const string CodeTemplatesFolder = "CodeTemplates";
            public const string EntityTypeFolder = "CSharpEntityType";
            public const string PartialsFolder = "Partials";
            public const string ClassFile = "Class.hbs";
            public const string ImportsFile = "Imports.hbs";
            public const string CtorFile = "Constructor.hbs";
            public const string PropertiesFile = "Properties.hbs";
        }
    }
}