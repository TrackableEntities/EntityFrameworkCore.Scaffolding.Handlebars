# Handlebars Scaffolding Templates Example

Demonstrates how to reverse engineer an existing database using the EF Core tooling with Handlebars templates.

## Prerequisites

- [Visual Studio 2019](https://www.visualstudio.com/downloads/) 16.4 or greater.
- The .[NET Core 3.1 SDK](https://www.microsoft.com/net/download/core).

## Database Setup

- Use SQL Server Management Studio to connect to SQL Server
    - The easiest is to use **LocalDb**, which is installed with Visual Studio.  
    Connect to: `(localdb)\MsSqlLocalDb`.
    - Create a new database named **NorthwindSlim**.
    - Download the `NorthwindSlim.sql` file from <https://github.com/TrackableEntities/northwind-slim>.
    - Unzip **NorthwindSlim.sql** and run the script to create tables and populate them with data.

## Setup

Add NuGet packages.  
    - `Microsoft.EntityFrameworkCore.SqlServer`
    - `Microsoft.EntityFrameworkCore.Design`
    - `EntityFrameworkCore.Scaffolding.Handlebars`

The `ScaffoldingDesignTimeServices` class is where the setup takes place.

```csharp
public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        // Add Handlebars scaffolding templates
        services.AddHandlebarsScaffolding(options =>
        {
            // Generate both context and entities
            options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;

            // Exclude some tables
            options.ExcludedTables = new List<string> { "Territory", "EmployeeTerritories" };

            // Add custom template data
            options.TemplateData = new Dictionary<string, object>
            {
                { "models-namespace", "ScaffoldingSample.Models" },
                { "base-class", "EntityBase" }
            };
        });

        // Register Handlebars helper
        var myHelper = (helperName: "my-helper", helperFunction: (Action<TextWriter, Dictionary<string, object>, object[]>) MyHbsHelper);

        // Register Handlebars block helper
        var ifCondHelper = (helperName: "ifCond", helperFunction: (Action<TextWriter, HelperOptions, Dictionary<string, object>, object[]>)MyHbsBlockHelper);

        // Add optional Handlebars helpers
        services.AddHandlebarsHelpers(myHelper);
        services.AddHandlebarsBlockHelpers(ifCondHelper);

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

    // Sample Handlebars block helper
    void MyHbsBlockHelper(TextWriter writer, HelperOptions options, Dictionary<string, object> context, object[] args)
    {
        var val1 = float.Parse(args[0].ToString());
        var val2 = float.Parse(args[2].ToString());

        switch (args[1].ToString())
        {
            case ">":
                if (val1 > val2)
                    options.Template(writer, context);
                else
                    options.Inverse(writer, context);
                break;
            case "=":
            case "==":
                if (val1 == val2)
                    options.Template(writer, context);
                else
                    options.Inverse(writer, context);
                break;
            case "<":
                if (val1 < val2)
                    options.Template(writer, context);
                else
                    options.Inverse(writer, context);
                break;
            case "!=":
            case "<>":
                if (val1 != val2)
                    options.Template(writer, context);
                else
                    options.Inverse(writer, context);
                break;
        }
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

A partial `NorthwindSlimContext` class has been added to a **NorthwindSlimContextPartial.cs** file with an implementation of the partial `OnModelCreatingPartial` method that adds a property value conversion.

```csharp
public partial class NorthwindSlimContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
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

- Install the global `dotnet ef` tool.
```
dotnet tool install --global dotnet-ef --version 3.1.0-*
```
- Open a command prompt at the **ScaffoldingSample** project root and execute:
```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
```
- Set the solution startup project to the **ScaffoldingSample.Api** project and press Ctrl+F5 to start the project without debugging. You should see data displayed from the Employee table.