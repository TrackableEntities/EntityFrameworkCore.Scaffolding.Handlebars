using System;
using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class HbsDbContextTemplateService : HbsTemplateService, IDbContextTemplateService
    {
        public Func<object, string> DbContextTemplate { get; private set; }

        public HbsDbContextTemplateService(ITemplateFileService fileService) : base(fileService)
        {
        }

        public virtual string GenerateDbContext(object data)
        {
            if (DbContextTemplate == null)
            {
                DbContextTemplate = CompileDbContextTemplate();
            }
            string dbContext = DbContextTemplate(data);
            return dbContext;
        }

        protected virtual Func<object, string> CompileDbContextTemplate()
        {
            var template = FileService.RetrieveFileContents(
                Constants.DbContextDirectory,
                Constants.DbContextTemplate + Constants.TemplateExtension);
            var entityTemplate = HandlebarsLib.Compile(template);
            return entityTemplate;
        }

        protected override IDictionary<string, string> GetPartialTemplates()
        {
            var importTemplate = FileService.RetrieveFileContents(
                Constants.DbContextPartialsDirectory,
                Constants.DbContextImportTemplate + Constants.TemplateExtension);
            var propertyTemplate = FileService.RetrieveFileContents(
                Constants.DbContextPartialsDirectory,
                Constants.DbContextDbSetsTemplate + Constants.TemplateExtension);

            var templates = new Dictionary<string, string>
            {
                {
                    Constants.DbContextImportTemplate.ToLower(),
                    importTemplate
                },
                {
                    Constants.DbContextDbSetsTemplate.ToLower(),
                    propertyTemplate
                },
            };
            return templates;
        }
    }
}