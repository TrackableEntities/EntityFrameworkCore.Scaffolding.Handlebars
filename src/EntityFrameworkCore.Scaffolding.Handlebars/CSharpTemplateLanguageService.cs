using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services to obtain C# template file information.
    /// </summary>
    public class CSharpTemplateLanguageService : ITemplateLanguageService
    {
        /// <summary>
        /// Get DbContext template file information.
        /// </summary>
        /// <returns>Dictionary of templates with file information.</returns>
        public Dictionary<string, TemplateFileInfo> GetDbContextTemplateFileInfo(ITemplateFileService fileService)
        {
            var result = new Dictionary<string, TemplateFileInfo>
            {
                {
                    Constants.DbContextTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.DbContextDirectory,
                        FileName = Constants.DbContextTemplate + Constants.TemplateExtension
                    }
                }
            };

            result = fileService.FindAllPartialTemplates(result, Constants.CSharpTemplateDirectories.DbContextPartialsDirectory);
            return result;
        }

        /// <summary>
        /// Get Entities template file information.
        /// </summary>
        /// <returns>Dictionary of templates with file information.</returns>
        public Dictionary<string, TemplateFileInfo> GetEntitiesTemplateFileInfo(ITemplateFileService fileService)
        {
            var result = new Dictionary<string, TemplateFileInfo>
            {
                {
                    Constants.EntityTypeTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypeDirectory,
                        FileName = Constants.EntityTypeTemplate + Constants.TemplateExtension
                    }
                },
            };

            result = fileService.FindAllPartialTemplates(result, Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory);
            return result;
        }
    }
}
