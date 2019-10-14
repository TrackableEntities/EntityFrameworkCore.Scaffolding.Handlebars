// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeDatabaseModelFactory : IDatabaseModelFactory
    {
        public DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options) 
            => throw new NotImplementedException();

        public DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options) 
            => throw new NotImplementedException();
    }
}