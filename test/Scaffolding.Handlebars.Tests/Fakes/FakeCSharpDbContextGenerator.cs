using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeCSharpDbContextGenerator : ICSharpDbContextGenerator
    {
        public string WriteCode(IModel model, string @namespace, string contextName, string connectionString, bool useDataAnnotations)
        {
            return string.Empty;
        }
    }
}