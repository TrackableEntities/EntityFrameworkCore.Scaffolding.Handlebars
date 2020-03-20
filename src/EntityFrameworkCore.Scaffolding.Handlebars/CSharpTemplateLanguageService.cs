﻿using System.Collections.Generic;
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
                //{
                //    Constants.EntityTypeImportTemplate,
                //    new TemplateFileInfo
                //    {
                //        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                //        FileName = Constants.EntityTypeImportTemplate + Constants.TemplateExtension
                //    }
                //},
                //{
                //    Constants.EntityTypeCtorTemplate,
                //    new TemplateFileInfo
                //    {
                //        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                //        FileName = Constants.EntityTypeCtorTemplate + Constants.TemplateExtension
                //    }
                //},
                //{
                //    Constants.EntityTypePropertyTemplate,
                //    new TemplateFileInfo
                //    {
                //        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                //        FileName = Constants.EntityTypePropertyTemplate + Constants.TemplateExtension
                //    }
                //},
                //{
                //    Constants.EntityTypePropertyTemplate + "item"   ,
                //    new TemplateFileInfo
                //    {
                //        RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                //        FileName = Constants.EntityTypePropertyTemplate + "item" + Constants.TemplateExtension
                //    }
                //},
            };
            
            foreach(var file in fileService.RetrieveAllFileNames(Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory))
            {
                result.Add(file, new TemplateFileInfo()
                {
                    RelativeDirectory = Constants.CSharpTemplateDirectories.EntityTypePartialsDirectory,
                    FileName = file + Constants.TemplateExtension
                });
            }


            //EntityFrameworkCore.Scaffolding.Handlebars.FileService.
            return result;
        }
    }
}
