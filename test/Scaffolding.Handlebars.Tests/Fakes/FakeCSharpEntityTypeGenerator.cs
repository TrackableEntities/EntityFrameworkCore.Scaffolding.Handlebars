using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeCSharpEntityTypeGenerator : ICSharpEntityTypeGenerator
    {
        public string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            return string.Empty;
        }
    }
}