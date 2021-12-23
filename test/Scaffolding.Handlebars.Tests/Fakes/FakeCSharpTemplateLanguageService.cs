using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeCSharpTemplateLanguageService : ITemplateLanguageService
    {
        private readonly string _entityTypeDirectory;
        private readonly string _entityTypePartialsDirectory;

        public FakeCSharpTemplateLanguageService(bool useAltTemplates)
        {
            _entityTypeDirectory = useAltTemplates ? "CSharpEntityTypeAlt" : "CodeTemplates/CSharpEntityType";
            _entityTypePartialsDirectory = _entityTypeDirectory + "/Partials";
        }

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

        public Dictionary<string, TemplateFileInfo> GetEntitiesTemplateFileInfo()
        {
            var result = new Dictionary<string, TemplateFileInfo>
            {
                {
                    Constants.EntityTypeTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = _entityTypeDirectory,
                        FileName = Constants.EntityTypeTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.EntityTypeImportTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = _entityTypePartialsDirectory,
                        FileName = Constants.EntityTypeImportTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.EntityTypeCtorTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = _entityTypePartialsDirectory,
                        FileName = Constants.EntityTypeCtorTemplate + Constants.TemplateExtension
                    }
                },
                {
                    Constants.EntityTypePropertyTemplate,
                    new TemplateFileInfo
                    {
                        RelativeDirectory = _entityTypePartialsDirectory,
                        FileName = Constants.EntityTypePropertyTemplate + Constants.TemplateExtension
                    }
                },
            };
            return result;
        }
    }
}
