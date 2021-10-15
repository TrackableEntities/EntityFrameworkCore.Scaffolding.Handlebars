using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scaffolding.Handlebars.Tests.Helpers;
using VerifyTests;
using VerifyXunit;

namespace Scaffolding.Handlebars.Tests
{
    [UsesVerify]
    public abstract class HbsScaffoldingGeneratorTests
    {
        protected NorthwindDbContextFixture Fixture { get; }

        protected string VerifyDirectory { get; }

        protected HbsScaffoldingGeneratorTests(NorthwindDbContextFixture fixture, string verifyDirectory)
        {
            Fixture = fixture;
            VerifyDirectory = verifyDirectory;
        }

        private VerifySettings GetSettings(string name, bool useDataAnnotations, bool nullableReferenceTypes, bool suppressOnConfiguring)
        {
            var settings = new VerifySettings();
            settings.UseDirectory(VerifyDirectory);
            settings.DisableRequireUniquePrefix();

            var fileName = new StringBuilder(name);
            fileName.Append("_useDataAnnotations=");
            fileName.Append(useDataAnnotations.ToString().ToLowerInvariant());
            if (name != "Context")
            {
                fileName.Append("_nullableReferenceTypes=");
                fileName.Append(nullableReferenceTypes.ToString().ToLowerInvariant());
            }
            else
            {
                if (suppressOnConfiguring)
                {
                    fileName.Append("_suppressOnConfiguring=true");
                }
                else
                {
                    fileName.Append(".");
                    fileName.Append(Fixture.ProviderName.Split(".").Last());
                }
            }
            settings.UseFileName(fileName.ToString());
            return settings;
        }

        protected Task VerifyContext(string value, bool useDataAnnotations, bool suppressOnConfiguring = false)
            => Verifier.Verify(value, GetSettings("Context", useDataAnnotations, nullableReferenceTypes: false, suppressOnConfiguring));

        protected Task VerifyCategory(string value, bool useDataAnnotations, bool nullableReferenceTypes = false)
            => Verifier.Verify(value, GetSettings("Category", useDataAnnotations, nullableReferenceTypes, suppressOnConfiguring: false));

        protected Task VerifyProduct(string value, bool useDataAnnotations, bool nullableReferenceTypes = false)
            => Verifier.Verify(value, GetSettings("Product", useDataAnnotations, nullableReferenceTypes, suppressOnConfiguring: false));
    }
}