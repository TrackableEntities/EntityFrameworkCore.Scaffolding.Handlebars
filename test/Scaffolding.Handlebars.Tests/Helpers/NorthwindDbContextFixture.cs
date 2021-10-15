using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Design.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Scaffolding.Handlebars.Tests.Contexts;

namespace Scaffolding.Handlebars.Tests.Helpers
{
    public class NorthwindDbContextFixture
    {
        public NorthwindDbContextFixture()
        {
            var userSqlServerString = Environment.GetEnvironmentVariable("Scaffolding.Handlebars.Tests.SqlServer");
            var useSqlServer = userSqlServerString != null && bool.TryParse(userSqlServerString, out var result) && result;

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<NorthwindDbContext>();
            if (useSqlServer)
            {
                dbContextOptionsBuilder.UseSqlServer(Constants.Connections.SqlServerConnection);
            }
            else
            {
                dbContextOptionsBuilder.UseSqlite(Constants.Connections.SqLiteConnection);
            }

            Context = new NorthwindDbContext(dbContextOptionsBuilder.Options);
            Context.Database.EnsureCreated(); // If login error, manually create NorthwindTest database
        }

        public NorthwindDbContext Context { get; }

        public string ConnectionString => Context.Database.GetConnectionString();

        public string ProviderName => Context.Database.ProviderName;

        [SuppressMessage("", "EF1001:internal API that supports the Entity Framework Core infrastructure")]
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            if (Context.Database.IsSqlServer())
            {
                new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
            }
            else if (Context.Database.IsSqlite())
            {
                new SqliteDesignTimeServices().ConfigureDesignTimeServices(services);
            }
            else
            {
                throw new NotSupportedException("Only SqlServer and Sqlite are currently supported.");
            }
        }
    }
}
