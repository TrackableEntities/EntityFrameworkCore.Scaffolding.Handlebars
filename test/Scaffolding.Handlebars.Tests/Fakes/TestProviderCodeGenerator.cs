// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

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

#pragma warning disable 672
        public override MethodCallCodeFragment GenerateUseProvider(string connectionString)
#pragma warning restore 672
            => new MethodCallCodeFragment("UseTestProvider", connectionString);
    }
}