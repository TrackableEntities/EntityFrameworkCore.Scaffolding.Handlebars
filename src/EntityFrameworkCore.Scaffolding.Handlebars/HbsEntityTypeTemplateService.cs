using System;
using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class HbsEntityTypeTemplateService : HbsTemplateService, IEntityTypeTemplateService
    {
        public Func<object, string> EntityTypeTemplate { get; private set; }

        public HbsEntityTypeTemplateService(ITemplateFileService fileService) : base(fileService)
        {
        }

        public virtual string GenerateEntityType(object data)
        {
            if (EntityTypeTemplate == null)
            {
                EntityTypeTemplate = CompileEntityTypeTemplate();
            }
            string entityType = EntityTypeTemplate(data);
            return entityType;
        }

        protected virtual Func<object, string> CompileEntityTypeTemplate()
        {
            var template = FileService.RetrieveFileContents(
                Constants.EntityTypeDirectory,
                Constants.EntityTypeTemplate + Constants.TemplateExtension);
            var entityTemplate = HandlebarsLib.Compile(template);
            return entityTemplate;
        }

        protected override IDictionary<string, string> GetPartialTemplates()
        {
            var ctorTemplate = FileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory,
                Constants.EntityTypeCtorTemplate + Constants.TemplateExtension);
            var importTemplate = FileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory,
                Constants.EntityTypeImportTemplate + Constants.TemplateExtension);
            var propertyTemplate = FileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory,
                Constants.EntityTypePropertyTemplate + Constants.TemplateExtension);

            var templates = new Dictionary<string, string>
            {
                {
                    Constants.EntityTypeCtorTemplate.ToLower(),
                    ctorTemplate
                },
                {
                    Constants.EntityTypeImportTemplate.ToLower(),
                    importTemplate
                },
                {
                    Constants.EntityTypePropertyTemplate.ToLower(),
                    propertyTemplate
                },
            };
            return templates;
        }
    }
}