using System;
using System.Collections.Generic;
using System.IO;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class HbsEntityTypeTemplateService : IEntityTypeTemplateService
    {
        private readonly ITemplateFileService _fileService;

        public Func<object, string> EntityTypeTemplate { get; private set; } 

        public HbsEntityTypeTemplateService(ITemplateFileService fileService)
        {
            _fileService = fileService;
        }

        public virtual void RegisterHelper(string helperName, Action<TextWriter, object, object[]> helper)
        {
            HandlebarsLib.RegisterHelper(helperName, (output, context, args) => helper(output, context, args));
        }

        public virtual void RegisterPartialTemplates()
        {
            var partialTemplates = GetPartialTemplates();
            foreach (var partialTemplate in partialTemplates)
            {
                HandlebarsLib.RegisterTemplate(partialTemplate.Key, partialTemplate.Value);
            }
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
            var template = _fileService.RetrieveFileContents(
                Constants.EntityTypeDirectory,
                Constants.EntityTypeTemplate + Constants.TemplateExtension);
            var entityTemplate = HandlebarsLib.Compile(template);
            return entityTemplate;
        }

        protected virtual IDictionary<string, string> GetPartialTemplates()
        {
            var ctorTemplate = _fileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory,
                Constants.EntityTypeCtorTemplate + Constants.TemplateExtension);
            var importTemplate = _fileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory,
                Constants.EntityTypeImportTemplate + Constants.TemplateExtension);
            var propertyTemplate = _fileService.RetrieveFileContents(
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