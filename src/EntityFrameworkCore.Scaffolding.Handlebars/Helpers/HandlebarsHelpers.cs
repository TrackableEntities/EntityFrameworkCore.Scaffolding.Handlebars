using HandlebarsDotNet;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Helpers
{
    public static class HandlebarsHelpers
    {
        public static HandlebarsHelper GetSpacesHelper()
        {
            return (writer, context, parameters) =>
            {
                var spaces = string.Empty;
                if (parameters.Length > 0
                    && parameters[0] is string param
                    && int.TryParse(param, out int count))
                {
                    for (int i = 0; i < count; i++)
                        spaces += " ";
                }
                writer.Write(spaces);
            };
        }
    }
}