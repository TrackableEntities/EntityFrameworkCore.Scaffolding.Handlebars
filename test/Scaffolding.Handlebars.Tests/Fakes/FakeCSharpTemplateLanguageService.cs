using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeCSharpTemplateLanguageService : ITemplateLanguageService
    {
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
