using System.IO;
using System.Reflection;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provides files to the template service from resources embedded in an assembly.
    /// </summary>
    public class EmbeddedResourceTemplateFileService : InMemoryTemplateFileService
    {
        private readonly Assembly _assembly;
        private readonly string _namespace;

        /// <summary>
        /// Constructor for the template file service.
        /// </summary>
        /// <param name="assembly">Assembly to get embedded resources from.</param>
        /// <param name="namespace">Namespace of the resource files; this is generally the default namespace of the assembly.</param>
        public EmbeddedResourceTemplateFileService(Assembly assembly, string @namespace)
        {
            _assembly = assembly;
            _namespace = @namespace;
            if (string.IsNullOrWhiteSpace(_namespace)) _namespace = _assembly.GetName().Name;
        }

        /// <inheritdoc />
        public override string RetrieveTemplateFileContents(string relativeDirectory, string fileName, string altRelativeDirectory = null)
        {
            string content;
            string resourceLocation = $"{_namespace}.{relativeDirectory.Replace('/', '.')}.{fileName}";
            using (var stream = _assembly.GetManifestResourceStream(resourceLocation))
            {
                using var reader = new StreamReader(stream);
                content = reader.ReadToEnd();
            }

            return content;
        }
    }
}
