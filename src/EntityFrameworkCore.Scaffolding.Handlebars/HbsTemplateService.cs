using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class HbsTemplateService : IHbsTemplateService
    {
        protected readonly ITemplateFileService FileService;

        public HbsTemplateService(ITemplateFileService fileService)
        {
            FileService = fileService;
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

        protected virtual IDictionary<string, string> GetPartialTemplates()
        {
            return new Dictionary<string, string>();
        }
    }
}