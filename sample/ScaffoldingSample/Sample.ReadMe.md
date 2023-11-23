# Handlebars Scaffolding Templates Example

Demonstrates how to reverse engineer an existing database using the EF Core tooling with Handlebars templates.

## Prerequisites

- [Visual Studio 2022](https://www.visualstudio.com/downloads/) 17.4 or greater.
- The .[NET SDK](https://www.microsoft.com/net/download/core).

## Windows Intel Setup

- Use SQL Server Management Studio to connect to SQL Server
- The easiest is to use **LocalDb**, which is installed with Visual Studio.
- Connect to: `(localdb)\MsSqlLocalDb`.
- Create a new database named **NorthwindSlim**.
- Download the `NorthwindSlim.sql` file from <https://github.com/TrackableEntities/northwind-slim>.
- Unzip **NorthwindSlim.sql** and run the script to create tables and populate them with data.

## MacOS arm64 Setup (M Series)

- Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Run Docker SQL Server instance for arm64

```
docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1433:1433 -d --name=sql mcr.microsoft.com/azure-sql-edge
```

- Add VS Code [Extension for SQL Server](https://marketplace.visualstudio.com/items?itemName=ms-mssql.mssql)
  - Connect with username `sa` and password `MyPass@word`
  - Enable trust server certificate when prompted
  - See [here](https://learn.microsoft.com/en-us/sql/tools/visual-studio-code/sql-server-develop-use-vscode?view=sql-server-ver16) for help connecting and writing commands and queries
- Create a new database named **NorthwindSlim**.
- Download the `NorthwindSlim.sql` file from <https://github.com/TrackableEntities/northwind-slim>.
- Unzip **NorthwindSlim.sql** and run the script to create tables and populate them with data.

## Project Setup

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
            .HasConversion<string>();

        modelBuilder.Entity<Customer>()
            .Property(e => e.Country)
            .HasConversion<string>();
    }
}
```

## Usage

- Install the global `dotnet ef` tool.
```
dotnet tool install --global dotnet-ef
```

- Open a command prompt at the **ScaffoldingSample** project root.
- *For Windows Intel:*

```
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
```

- *For MacOS arm64:*

```
dotnet ef dbcontext scaffold "Server=localhost; Database=NorthwindSlim; User ID=sa;Password=MyPass@word; TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
```

## Web API Sample Project

- To run on macOS with arm64, set connection string with Secret Manager

```
dotnet user-secrets set "ConnectionStrings:NorthwindSlimContext" "Server=localhost; Database=NorthwindSlim; User ID=sa;Password=MyPass@word; TrustServerCertificate=True;"
```

- Set the solution startup project to the **ScaffoldingSample.Api** project and press Ctrl+F5 to start the project without debugging. You should see data displayed from the Employee table.