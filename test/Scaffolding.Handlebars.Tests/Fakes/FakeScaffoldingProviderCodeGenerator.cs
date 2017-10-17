// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeScaffoldingProviderCodeGenerator : IScaffoldingProviderCodeGenerator
    {
        public string GenerateUseProvider(string connectionString, string language)
        {
            throw new NotImplementedException();
        }

        public TypeScaffoldingInfo GetTypeScaffoldingInfo(DatabaseColumn column)
        {
            throw new NotImplementedException();
        }
    }
}
