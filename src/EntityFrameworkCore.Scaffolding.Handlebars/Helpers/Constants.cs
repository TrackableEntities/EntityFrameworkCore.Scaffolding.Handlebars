namespace EntityFrameworkCore.Scaffolding.Handlebars.Helpers
{
    public static class Constants
    {
        public const string SpacesHelper = "spaces";

        public const string TemplateExtension = ".hbs";

        public const string CodeTemplatesDirectory = "CodeTemplates";
        public const string EntityTypeDirectory = CodeTemplatesDirectory + "/CSharpEntityType";
        public const string EntityTypePartialsDirectory = EntityTypeDirectory + "/Partials";

        public const string EntityTypeTemplate = "CSharpEntityType";
        public const string EntityTypeImportTemplate = "Import";
        public const string EntityTypePropertyTemplate = "Property";
    }
}