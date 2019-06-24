# Handlebars Scaffolding TypeScript Templates Example

Demonstrates how to reverse engineer an existing database using the EF Core tooling with Handlebars templates.

## Prerequisites

- [Visual Studio 2017](https://www.visualstudio.com/downloads/) 15.9 or greater.
- The .[NET Core 2.2 SDK](https://www.microsoft.com/net/download/core) (version 2.2.100 or greater).

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
        // Generate both context and entities
        var options = ReverseEngineerOptions.DbContextAndEntities;

        // Generate TypeScript files
        var language = LanguageOptions.TypeScript;

        // Add Handlebars scaffolding templates
        services.AddHandlebarsScaffolding(options, language);
    }
}
```

- Set the `language` parameter to `LanguageOptions.TypeScript` in order to generate TypeScript files.

## Usage

- Open a command prompt at the **ScaffoldingSample** project root and execute:

```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -f
```
