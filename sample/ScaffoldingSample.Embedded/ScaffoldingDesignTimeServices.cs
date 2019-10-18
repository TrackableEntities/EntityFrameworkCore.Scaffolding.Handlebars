using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

/*
Run a 'dotnet ef dbcontext scaffold' command to reverse engineer classes from an existing database.
For example:
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
*/
namespace ScaffoldingSample.Embedded
{
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            // Get templates assembly
            var templatesAssembly = Assembly.Load("TemplatesAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

            // Add Handlebars scaffolding using embedded templates templates
            services.AddHandlebarsScaffolding(options => options.EmbeddedTemplatesAssembly = templatesAssembly);
        }
    }
}
