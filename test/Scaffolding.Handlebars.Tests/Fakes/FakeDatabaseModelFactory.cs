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