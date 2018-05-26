# Handlebars Scaffolding Templates Example

Demonstrates how to reverse engineer an existing database using the EF Core tooling with Handlebars templates.

## Prerequisites

- [Visual Studio 2017](https://www.visualstudio.com/downloads/) 15.8 or greater.
- The .[NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) (version 2.1.3 or greater).

## Database Setup

- Use SQL Server Management Studio to connect to SQL Server
    - The easiest is to use **LocalDb**, which is installed with Visual Studio.  
    Connect to: `(localdb)\MsSqlLocalDb`.
    - Create a new database named **NorthwindSlim**.
    - Download the data file from <http://bit.ly/northwindslim>.
    - Unzip **NorthwindSlim.sql** and run the script to create tables and populate them with data.

## Setup

The `ScaffoldingDesignTimeServices` class is where the setup takes place.

```csharp
public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        // Generate both context and entitites
        var options = ReverseEngineerOptions.DbContextAndEntities;

        // Register Handlebars helper
        var myHelper = (helperName: "my-helper", helperFunction: (Action<TextWriter, object, object[]>) MyHbsHelper);

        // Add Handlebars scaffolding templates
        services.AddHandlebarsScaffolding(options, myHelper);
    }

    // Sample Handlebars helper
    void MyHbsHelper(TextWriter writer, object context, object[] parameters)
    {
        writer.Write("// My Handlebars Helper");
    }
}
```
- To use Handlebars helper defined above, add the following to any of the .hbs files within the CodeTemplates folder: `{{my-helper}}`

## Usage

- Open a command prompt at the project root and execute:

```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
```