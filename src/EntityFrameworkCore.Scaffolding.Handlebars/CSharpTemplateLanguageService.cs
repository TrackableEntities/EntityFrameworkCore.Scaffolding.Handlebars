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
        public Dictionary<string, TemplateFileInfo> GetDbContextTemplateFileInfo()
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
                },
                {
                    Constants.DbContextImportTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.DbContextPartialsDirectory,
                        FileName = Constants.DbContextImportTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.DbContextCtorTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.DbContextPartialsDirectory,
                        FileName = Constants.DbContextCtorTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.DbContextOnConfiguringTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.DbContextPartialsDirectory,
                        FileName = Constants.DbContextOnConfiguringTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.DbContextDbSetsTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.DbContextPartialsDirectory,
                        FileName = Constants.DbContextDbSetsTemplate + Constants.TemplateExtension
                    }
                },
            };
            return result;
        }

        /// <summary>
        /// Get Entities template file information.
        /// </summary>
        /// <returns>Dictionary of templates with file information.</returns>
        public Dictionary<string, TemplateFileInfo> GetEntitiesTemplateFileInfo()
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
                {
                    Constants.EntityTypeImportTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                        FileName = Constants.EntityTypeImportTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.EntityTypeCtorTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                        FileName = Constants.EntityTypeCtorTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.EntityTypePropertyTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                        FileName = Constants.EntityTypePropertyTemplate + Constants.TemplateExtension
                    }
                },
            };
            return result;
        }
    }
}
