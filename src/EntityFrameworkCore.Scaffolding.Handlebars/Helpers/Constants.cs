namespace EntityFrameworkCore.Scaffolding.Handlebars.Helpers
{
    /// <summary>
    /// Constants used by the Handlebars scaffolding generator.
    /// </summary>
    public static class Constants
    {
        /// <summary>Spaces Handlebars helper.</summary>
        public const string SpacesHelper = "spaces";

        /// <summary>Handlebars templates file extension.</summary>
        public const string TemplateExtension = ".hbs";

        /// <summary>Code templates folder.</summary>
        public const string CodeTemplatesDirectory = "CodeTemplates";

        /// <summary>Entity type template.</summary>
        public const string EntityTypeTemplate = "Class";

        /// <summary>Entity type constructor template.</summary>
        public const string EntityTypeCtorTemplate = "Constructor";

        /// <summary>Entity type imports template.</summary>
        public const string EntityTypeImportTemplate = "Imports";

        /// <summary>Entity type properties template.</summary>
        public const string EntityTypePropertyTemplate = "Properties";

        /// <summary>DbContext template.</summary>
        public const string DbContextTemplate = "DbContext";

        /// <summary>DbContext imports template.</summary>
        public const string DbContextImportTemplate = "DbImports";

        /// <summary>DbContext constructor template.</summary>
        public const string DbContextCtorTemplate = "DbConstructor";

        /// <summary>DbContext OnConfiguring template.</summary>
        public const string DbContextOnConfiguringTemplate = "DbOnConfiguring";

        /// <summary>DbContext DbSets template.</summary>
        public const string DbContextDbSetsTemplate = "DbSets";

        /// <summary>C# template folders.</summary>
        public static class CSharpTemplateDirectories
        {
            /// <summary>DbContext C# template folder.</summary>
            public const string DbContextDirectory = CodeTemplatesDirectory + "/CSharpDbContext";

            /// <summary>DbContext partial C# templates folder.</summary>
            public const string DbContextPartialsDirectory = DbContextDirectory + "/Partials";

            /// <summary>Entity type C# template folder.</summary>
            public const string EntityTypeDirectory = CodeTemplatesDirectory + "/CSharpEntityType";

            /// <summary>Entity type partial C# templates folder.</summary>
            public const string EntityTypePartialsDirectory = EntityTypeDirectory + "/Partials";
        }

        /// <summary>TypeScript template folders.</summary>
        public static class TypeScriptTemplateDirectories
        {
            /// <summary>DbContext TypeScript template folder.</summary>
            public const string DbContextDirectory = CodeTemplatesDirectory + "/TypeScriptDbContext";

            /// <summary>DbContext partial TypeScript templates folder.</summary>
            public const string DbContextPartialsDirectory = DbContextDirectory + "/Partials";

            /// <summary>Entity type TypeScript template folder.</summary>
            public const string EntityTypeDirectory = CodeTemplatesDirectory + "/TypeScriptEntityType";

            /// <summary>Entity type partial TypeScript templates folder.</summary>
            public const string EntityTypePartialsDirectory = EntityTypeDirectory + "/Partials";
        }
    }
}