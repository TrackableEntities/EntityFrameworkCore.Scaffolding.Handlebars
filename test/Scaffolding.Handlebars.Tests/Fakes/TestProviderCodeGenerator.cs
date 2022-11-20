using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class TestProviderCodeGenerator : ProviderCodeGenerator
    {
        public TestProviderCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override MethodCallCodeFragment GenerateUseProvider(string connectionString, MethodCallCodeFragment providerOptions)
#pragma warning disable CS0618 // Type or member is obsolete
            => new MethodCallCodeFragment("UseTestProvider", connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}