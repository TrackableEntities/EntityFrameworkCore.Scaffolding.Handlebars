# Handlebars Scaffolding Templates Example

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

        // Register Handlebars helper
        var myHelper = (helperName: "my-helper", helperFunction: (Action<TextWriter, object, object[]>) MyHbsHelper);

        // Add Handlebars scaffolding templates
        services.AddHandlebarsScaffolding(options);

        // Add optional Handlebars helpers
        services.AddHandlebarsHelpers(myHelper);

        // Add Handlebars transformer for Country property
        services.AddHandlebarsTransformers(
            propertyTransformer: e =>
                e.PropertyName == "Country"
                    ? new EntityPropertyInfo("Country", e.PropertyName)
                    : new EntityPropertyInfo(e.PropertyType, e.PropertyName));

        // Add optional Handlebars transformers
        //services.AddHandlebarsTransformers(
        //    entityNameTransformer: n => n + "Foo",
        //    entityFileNameTransformer: n => n + "Foo",
        //    constructorTransformer: e => new EntityPropertyInfo(e.PropertyType + "Foo", e.PropertyName + "Foo"),
        //    propertyTransformer: e => new EntityPropertyInfo(e.PropertyType, e.PropertyName + "Foo"),
        //    navPropertyTransformer: e => new EntityPropertyInfo(e.PropertyType + "Foo", e.PropertyName + "Foo"));
    }

    // Sample Handlebars helper
    void MyHbsHelper(TextWriter writer, Dictionary<string, object> context, object[] parameters)
    {
        writer.Write("// My Handlebars Helper");
    }
}
```
- To use Handlebars helper defined above, add the following to any of the .hbs files within the CodeTemplates folder: `{{my-helper}}`

A transformer function is added to convert the property type of the `Country` property from `string` to a `Country` `enum` that has been added to the project. More countries from the Customer table may be added.
- The commented code for `AddHandlebarsTransformers` provides an example of using all the available transformers to append `"Foo"` to the class and property names of each entity.

```csharp
public enum Country
{
    UK = 1,
    USA = 2
}
```

A partial `NorthwindSlimContext` class has been added to a **NorthwindSlimContextExt.cs** file with an implementation of the partial `OnModelCreatingExt` method that adds a property value conversion.

```csharp
public partial class NorthwindSlimContext
{
    partial void OnModelCreatingExt(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .Property(e => e.Country)
            .HasConversion(
                v => v.ToString(),
                v => (Country)Enum.Parse(typeof(Country), v));

        modelBuilder.Entity<Customer>()
            .Property(e => e.Country)
            .HasConversion(
                v => v.ToString(),
                v => (Country)Enum.Parse(typeof(Country), v));
    }
}
```

## Usage

- Open a command prompt at the project root and execute:

```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
```