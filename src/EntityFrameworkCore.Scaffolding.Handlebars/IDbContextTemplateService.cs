using System;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public interface IDbContextTemplateService : IHbsTemplateService
    {
        Func<object, string> DbContextTemplate { get; }
        string GenerateDbContext(object data);
    }
}