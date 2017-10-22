using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class NullCSharpEntityTypeGenerator : ICSharpEntityTypeGenerator
    {
        public string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            return string.Empty;
        }
    }
}