using System;
using System.IO;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public interface IHbsTemplateService
    {
        void RegisterPartialTemplates();
        void RegisterHelper(string helperName, Action<TextWriter, object, object[]> helper);
    }
}