using System.Collections.Generic;
using System.Globalization;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to generate the DbContext class using Handlebars templates.
    /// </summary>
    public class HbsDbContextTemplateService : HbsTemplateService, IDbContextTemplateService
    {
        private Dictionary<string, TemplateFileInfo> DbContextTemplateFiles { get; }

        /// <summary>
        /// DbContext template.
        /// </summary>
        public HandlebarsTemplate<object, object> DbContextTemplate { get; private set; }

        /// <summary>
        /// Constructor for the DbContext template service.
        /// </summary>
        /// <param name="fileService">Template file service.</param>
        /// <param name="languageService">Template language service.</param>
        public HbsDbContextTemplateService(ITemplateFileService fileService,
            ITemplateLanguageService languageService) : base(fileService, languageService)
        {
            DbContextTemplateFiles = LanguageService.GetDbContextTemplateFileInfo();
        }

        /// <summary>
        /// Generate DbContext class.
        /// </summary>
        /// <param name="data">Data used to generate DbContext class.</param>
        /// <returns>Generated DbContext class.</returns>
        public virtual string GenerateDbContext(object data)
        {
            if (DbContextTemplate == null)
            {
                DbContextTemplate = CompileDbContextTemplate();
            }
            string dbContext = DbContextTemplate(data);
            return dbContext;
        }

        /// <summary>
        /// Compile the DbContext template.
        /// </summary>
        /// <param name="language">Language option.</param>
        /// <returns>DbContext template.</returns>
        protected virtual HandlebarsTemplate<object, object> CompileDbContextTemplate(
            LanguageOptions language = LanguageOptions.CSharp)
        {
            DbContextTemplateFiles.TryGetValue(Constants.DbContextTemplate, out TemplateFileInfo contextFile);
            var contextTemplateFile = FileService.RetrieveTemplateFileContents(
                contextFile.RelativeDirectory, contextFile.FileName);
            var contextTemplate = HandlebarsLib.Compile(contextTemplateFile);
            return contextTemplate;
        }

        /// <summary>
        /// Get DbContext partial templates.
        /// </summary>
        /// <param name="language">Language option.</param>
        /// <returns>Partial templates.</returns>
        protected override IDictionary<string, string> GetPartialTemplates(
            LanguageOptions language = LanguageOptions.CSharp)
        {
            DbContextTemplateFiles.TryGetValue(Constants.DbContextImportTemplate, out TemplateFileInfo importFile);
            var importTemplateFile = FileService.RetrieveTemplateFileContents(
                importFile.RelativeDirectory, importFile.FileName);

            DbContextTemplateFiles.TryGetValue(Constants.DbContextCtorTemplate, out TemplateFileInfo ctorFile);
            var ctorTemplateFile = FileService.RetrieveTemplateFileContents(
                ctorFile.RelativeDirectory, ctorFile.FileName);

            DbContextTemplateFiles.TryGetValue(Constants.DbContextOnConfiguringTemplate, out TemplateFileInfo onConfiguringFile);
            var onConfiguringTemplateFile = FileService.RetrieveTemplateFileContents(
                ctorFile.RelativeDirectory, onConfiguringFile.FileName);

            DbContextTemplateFiles.TryGetValue(Constants.DbContextDbSetsTemplate, out TemplateFileInfo propertyFile);
            var propertyTemplateFile = FileService.RetrieveTemplateFileContents(
                propertyFile.RelativeDirectory, propertyFile.FileName);

            var templates = new Dictionary<string, string>
            {
                {
                    Constants.DbContextImportTemplate.ToLower(CultureInfo.InvariantCulture),
                    importTemplateFile
                },
                {
                    Constants.DbContextCtorTemplate.ToLower(CultureInfo.InvariantCulture),
                    ctorTemplateFile
                },
                {
                    Constants.DbContextOnConfiguringTemplate.ToLower(CultureInfo.InvariantCulture),
                    onConfiguringTemplateFile
                },
                {
                    Constants.DbContextDbSetsTemplate.ToLower(CultureInfo.InvariantCulture),
                    propertyTemplateFile
                },
            };
            return templates;
        }
    }
}