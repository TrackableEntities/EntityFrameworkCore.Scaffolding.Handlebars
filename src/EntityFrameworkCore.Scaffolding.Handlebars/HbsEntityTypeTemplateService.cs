using System.Collections.Generic;
using System.Globalization;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to generate entity type classes using Handlebars templates.
    /// </summary>
    public class HbsEntityTypeTemplateService : HbsTemplateService, IEntityTypeTemplateService
    {
        private Dictionary<string, TemplateFileInfo> EntitiesTemplateFiles { get; }

        /// <summary>
        /// Entity type template.
        /// </summary>
        public HandlebarsTemplate<object, object> EntityTypeTemplate { get; private set; }

        /// <summary>
        /// Constructor for entity type template service.
        /// </summary>
        /// <param name="fileService">Template file service.</param>
        /// <param name="languageService">Template language service.</param>
        public HbsEntityTypeTemplateService(ITemplateFileService fileService,
            ITemplateLanguageService languageService) : base(fileService, languageService)
        {
            EntitiesTemplateFiles = LanguageService.GetEntitiesTemplateFileInfo();
        }

        /// <summary>
        /// Generate entity type class.
        /// </summary>
        /// <param name="data">Data used to generate entity type class.</param>
        /// <returns>Generated entity type class.</returns>
        public virtual string GenerateEntityType(object data)
        {
            if (EntityTypeTemplate == null)
            {
                EntityTypeTemplate = CompileEntityTypeTemplate();
            }
            string entityType = EntityTypeTemplate(data);
            return entityType;
        }

        /// <summary>
        /// Compile entity type template.
        /// </summary>
        /// <param name="language">Language option.</param>
        /// <returns>Entity type template.</returns>
        protected virtual HandlebarsTemplate<object, object> CompileEntityTypeTemplate(
            LanguageOptions language = LanguageOptions.CSharp)
        {
            EntitiesTemplateFiles.TryGetValue(Constants.EntityTypeTemplate, out TemplateFileInfo classFile);
            var entityTemplateFile = FileService.RetrieveTemplateFileContents(
                classFile.RelativeDirectory, classFile.FileName);
            var entityTemplate = HandlebarsLib.Compile(entityTemplateFile);
            return entityTemplate;
        }

        /// <summary>
        /// Get partial templates.
        /// </summary>
        /// <param name="language">Language option.</param>
        /// <returns>Partial templates.</returns>
        protected override IDictionary<string, string> GetPartialTemplates(
            LanguageOptions language = LanguageOptions.CSharp)
        {
            EntitiesTemplateFiles.TryGetValue(Constants.EntityTypeCtorTemplate, out TemplateFileInfo ctorFile);
            var ctorTemplateFile = FileService.RetrieveTemplateFileContents(
                ctorFile.RelativeDirectory, ctorFile.FileName);

            EntitiesTemplateFiles.TryGetValue(Constants.EntityTypeImportTemplate, out TemplateFileInfo importFile);
            var importTemplateFile = FileService.RetrieveTemplateFileContents(
                importFile.RelativeDirectory, importFile.FileName);

            EntitiesTemplateFiles.TryGetValue(Constants.EntityTypePropertyTemplate, out TemplateFileInfo propertyFile);
            var propertyTemplateFile = FileService.RetrieveTemplateFileContents(
                propertyFile.RelativeDirectory, propertyFile.FileName);

            var templates = new Dictionary<string, string>
            {
                {
                    Constants.EntityTypeCtorTemplate.ToLower(CultureInfo.InvariantCulture),
                    ctorTemplateFile
                },
                {
                    Constants.EntityTypeImportTemplate.ToLower(CultureInfo.InvariantCulture),
                    importTemplateFile
                },
                {
                    Constants.EntityTypePropertyTemplate.ToLower(CultureInfo.InvariantCulture),
                    propertyTemplateFile
                },
            };
            return templates;
        }
    }
}