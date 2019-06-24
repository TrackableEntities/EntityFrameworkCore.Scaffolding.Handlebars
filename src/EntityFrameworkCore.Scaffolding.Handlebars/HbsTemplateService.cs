using Microsoft.EntityFrameworkCore.Design;
using System.Collections.Generic;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to generate classes using Handlebars templates.
    /// </summary>
    public abstract class HbsTemplateService : IHbsTemplateService
    {
        /// <summary>
        /// Template file service.
        /// </summary>
        protected readonly ITemplateFileService FileService;

        /// <summary>
        /// Template language service.
        /// </summary>
        protected readonly ITemplateLanguageService LanguageService;

        /// <summary>
        /// Constructor for the template service.
        /// </summary>
        /// <param name="fileService">Template file service.</param>
        /// <param name="languageService">Template language service.</param>
        protected HbsTemplateService(ITemplateFileService fileService,
            ITemplateLanguageService languageService)
        {
            FileService = fileService;
            LanguageService = languageService;
        }

        /// <summary>
        /// Register partial templates.
        /// </summary>
        public virtual void RegisterPartialTemplates()
        {
            var partialTemplates = GetPartialTemplates();
            foreach (var partialTemplate in partialTemplates)
            {
                HandlebarsLib.RegisterTemplate(partialTemplate.Key, partialTemplate.Value);
            }
        }

        /// <summary>
        /// Get partial templates.
        /// </summary>
        /// <param name="language">Language option.</param>
        /// <returns>Partial templates.</returns>
        protected abstract IDictionary<string, string> GetPartialTemplates(
            LanguageOptions language = LanguageOptions.CSharp);
    }
}