using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scaffolding.Handlebars.Tests.Contexts;

namespace Scaffolding.Handlebars.Tests.Helpers;

public class DatabaseFixture : IDisposable
{
    public IConfiguration Configuration { get; private set; }
    public string ConnectionString { get; private set; }
    public NorthwindDbContext NorthwindDbContext { get; private set; }

    private DbConnection _connection;
    
    public void Initialize()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("7cd5132a-1f06-4001-b62c-51cdcbe38dbf")
            .AddEnvironmentVariables()
            .Build();

        ConnectionString = Configuration.GetConnectionString(Constants.Connections.NorthwindTest);
        
         var options = new DbContextOptionsBuilder<NorthwindDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
         NorthwindDbContext = new NorthwindDbContext(options);
         
         // If login error, manually create NorthwindTest database
         NorthwindDbContext.Database.EnsureCreated();
         _connection = NorthwindDbContext.Database.GetDbConnection();
    }

    public void Dispose()
    {
        if (_connection != null && _connection.State != ConnectionState.Closed)
            _connection.Close();
    }
}