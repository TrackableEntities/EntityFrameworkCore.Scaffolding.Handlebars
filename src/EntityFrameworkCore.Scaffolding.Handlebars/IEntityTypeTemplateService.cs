using System.Collections.Generic;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public interface IEntityTypeTemplateService
    {
        string GetEntityTypeTemplate();
        IDictionary<string, string> GetEntityTypePartialTemplates();
    }
}