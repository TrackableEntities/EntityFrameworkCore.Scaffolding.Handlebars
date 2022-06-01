using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Helpers
{
    internal static class TableExcluder
    {
        public static bool IsExcluded(HandlebarsScaffoldingOptions options, IEntityType type)
        {
            if (options?.ExcludedTables?.Any() == true)
            {
                var schema = type.GetSchema() ?? type.GetViewSchema();
                var table = type.GetTableName() ?? type.GetViewName();

                var name = $"{schema}.{table}";

                foreach (var exclusion in options?.ExcludedTables)
                {
                    // For backwards compatibility, if there is no ".", then we'll assume it is a table/view to be exluded
                    // and add the schema wildcard
                    var pattern = exclusion.Contains('.')
                        ? exclusion
                        : $"*.{exclusion}";

                    // Simple glob emulation, credits to:
                    // https://stackoverflow.com/questions/188892/glob-pattern-matching-in-net
                    pattern = "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
                    if (Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        return true;
                }
            }

            return false;
        }
    }
}
