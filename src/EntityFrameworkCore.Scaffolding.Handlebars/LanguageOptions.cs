using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Design
{
    /// <summary>
    /// Language options for reverse engineering classes from an existing database.
    /// </summary>
    public interface ILanguageOptions
    {
        /// <summary>DbContext language template folder.</summary>
        string DbContextDirectory { get; set; }
        /// <summary>DbContext partial language templates folder.</summary>
        string DbContextPartialsDirectory { get; set; }
        /// <summary>Entity type language template folder.</summary>
        string EntityTypeDirectory { get; set; }
        /// <summary>Entity type partial language templates folder.</summary>
        string EntityTypePartialsDirectory { get; set; }
        /// <summary> language file extension. </summary>
        string FileExtension { get; set; }
        /// <summary>  Add Triple Slash To Comments </summary>
        bool AddTripleSlashToComments { get; }
        /// <summary> Property Name Conversion function</summary>
        Func<string, string> PropertyNameConversion { get; }
        /// <summary> Type Name Conversion funcction </summary>
        Func<string,string> TypeNameConversion { get; }

        /// <summary> Entity Type Import List Generator</summary>
        Func<IEntityType, List<Dictionary<string, object>>> EntityTypeImportListGenerator { get; }
    }

    /// <summary>
    /// Language options for reverse engineering classes from an existing database.
    /// </summary>
    public class LanguageOptions : ILanguageOptions
    {
        /// <summary>
        /// C# language.
        /// </summary>
        public static LanguageOptions CSharp => new LanguageOptions
        {
            DbContextDirectory = Constants.CSharpTemplate.DbContextDirectory,
            DbContextPartialsDirectory = Constants.CSharpTemplate.DbContextPartialsDirectory,
            EntityTypeDirectory = Constants.CSharpTemplate.EntityTypeDirectory,
            EntityTypePartialsDirectory = Constants.CSharpTemplate.EntityTypePartialsDirectory,
            FileExtension = Constants.CSharpTemplate.FileExtension
        };

        /// <summary>
        /// TypeScript language.
        /// </summary>
        public static LanguageOptions TypeScript => new LanguageOptions
        {
            DbContextDirectory = Constants.TypeScriptTemplate.DbContextDirectory,
            DbContextPartialsDirectory = Constants.TypeScriptTemplate.DbContextPartialsDirectory,
            EntityTypeDirectory = Constants.TypeScriptTemplate.EntityTypeDirectory,
            EntityTypePartialsDirectory = Constants.TypeScriptTemplate.EntityTypePartialsDirectory,
            FileExtension = Constants.TypeScriptTemplate.FileExtension,
            AddTripleSlashToComments = false,
            PropertyNameConversion = TypeScriptHelper.ToCamelCase,
            TypeNameConversion = TypeScriptHelper.TypeName,
            EntityTypeImportListGenerator = (entityType) =>
            {
                var imports = new List<Dictionary<string, object>>();

                var sortedNavigations = entityType.GetNavigations()
                    .OrderBy(n => n.IsOnDependent ? 0 : 1)
                    .ThenBy(n => n.IsCollection ? 1 : 0)
                    .Distinct();
                
                if (sortedNavigations.Any())
                {

                    foreach (var navigation in sortedNavigations)
                    {
                        imports.Add(new Dictionary<string, object> { { "import", navigation.TargetEntityType.Name } });
                    }
                }
                return imports;
            }
        };


        /// <summary>DbContext language template folder.</summary>
        public string DbContextDirectory { get; set; }

        /// <summary>DbContext partial language templates folder.</summary>
        public string DbContextPartialsDirectory { get; set; }

        /// <summary>Entity type language template folder.</summary>
        public string EntityTypeDirectory { get; set; }

        /// <summary>Entity type partial language templates folder.</summary>
        public string EntityTypePartialsDirectory { get; set; }

        /// <summary> language file extension. </summary>
        public string FileExtension { get; set; }

        /// <summary>  Add Triple Slash To Comments </summary>
        public bool AddTripleSlashToComments { get; set; } = true;

        /// <summary> Property Name Conversion function</summary>
        public Func<string, string> PropertyNameConversion { get; set; } = (s) => s;

        /// <summary> Type Name Conversion funcction </summary>
        public Func<string, string> TypeNameConversion { get; set; } = (s) => s;

        /// <summary> Entity Type Import List Generator</summary>
        public Func<IEntityType, List<Dictionary<string, object>>> EntityTypeImportListGenerator { get; set; } = (entityType) =>
        {
            var imports = new List<Dictionary<string, object>>();

            foreach (var ns in entityType.GetProperties()
                .SelectMany(p => p.ClrType.GetNamespaces())
                .Where(ns => ns != "System" && ns != "System.Collections.Generic")
                .Distinct()
#pragma warning disable EF1001 // Internal EF Core API usage.
                    .OrderBy(x => x, new Internal.NamespaceComparer()))
#pragma warning restore EF1001 // Internal EF Core API usage.
            {
                imports.Add(new Dictionary<string, object> { { "import", ns } });
            }
            return imports;
        };
    }
}