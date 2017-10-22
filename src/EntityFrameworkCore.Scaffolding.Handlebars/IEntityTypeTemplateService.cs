using System;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public interface IEntityTypeTemplateService : IHbsTemplateService
    {
        Func<object, string> EntityTypeTemplate { get; }
        string GenerateEntityType(object data);
    }
}