using System;
using System.IO;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public interface IEntityTypeTemplateService
    {
        void RegisterPartialTemplates();
        Func<object, string> EntityTypeTemplate { get; }
        void RegisterHelper(string helperName, Action<TextWriter, object, object[]> helper);
        string GenerateEntityType(object data);
    }
}