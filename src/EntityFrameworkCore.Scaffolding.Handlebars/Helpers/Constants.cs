namespace EntityFrameworkCore.Scaffolding.Handlebars.Helpers
{
    public static class Constants
    {
        public const string SpacesHelper = "spaces";

        public const string TemplateExtension = ".hbs";

        public const string CodeTemplatesDirectory = "CodeTemplates";
        public const string EntityTypeDirectory = CodeTemplatesDirectory + "/CSharpEntityType";
        public const string EntityTypePartialsDirectory = EntityTypeDirectory + "/Partials";

        public const string EntityTypeTemplate = "Class";
        public const string EntityTypeCtorTemplate = "Constructor";
        public const string EntityTypeImportTemplate = "Imports";
        public const string EntityTypePropertyTemplate = "Properties";
    }
}