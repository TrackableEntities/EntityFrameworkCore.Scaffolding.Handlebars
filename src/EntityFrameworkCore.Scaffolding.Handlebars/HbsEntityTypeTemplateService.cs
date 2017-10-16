using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class HbsEntityTypeTemplateService : IEntityTypeTemplateService
    {
        private readonly IFileService _fileService;

        public HbsEntityTypeTemplateService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public string GetEntityTypeTemplate()
        {
            var template = _fileService.RetrieveFileContents(
                Constants.EntityTypeDirectory, Constants.EntityTypeTemplate);
            return template;
        }

        public IDictionary<string, string> GetEntityTypePartialTemplates()
        {
            var importTemplate = _fileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory, Constants.EntityTypeImportTemplate);
            var propertyTemplate = _fileService.RetrieveFileContents(
                Constants.EntityTypePartialsDirectory, Constants.EntityTypePropertyTemplate);

            var templates = new Dictionary<string, string>
            {
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