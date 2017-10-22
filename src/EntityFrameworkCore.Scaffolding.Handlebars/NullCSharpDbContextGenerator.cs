using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class NullCSharpDbContextGenerator : ICSharpDbContextGenerator
    {
        public string WriteCode(IModel model, string @namespace, string contextName, string connectionString, bool useDataAnnotations)
        {
            return string.Empty;
        }
    }
}