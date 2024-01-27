# Entity Framework Core Scaffolding with Handlebars

_Scaffold EF Core models using Handlebars templates._

- Uses [Handlebars.NET](https://github.com/rexm/Handlebars.Net) to compile [Handlebars](http://handlebarsjs.com) templates when generating models with the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) scaffolding tools.
 
## EF Core Community Standup

View the [EF Core Community Standup](https://youtu.be/6Ux7EpgiWXE) episode featuring this framework for scaffolding entities with Handlebars templates. The demos for the episode can be found on this GitHub [repo](https://github.com/TrackableEntities/ef-core-community-handlebars).

## Contributing

Before creating a pull request, please refer to the [Contributing Guidelines](https://github.com/TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars/blob/master/.github/CONTRIBUTING.md).

## Prerequisites

- [Visual Studio 2022](https://www.visualstudio.com/downloads/) or greater, [JetBrains Rider](https://www.jetbrains.com/rider) 2022.2 or greater.
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet) or greater.
- [EF Core CLI 8.0](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) or greater.
  - Install global `dotnet-ef` tool.
    ```
    dotnet tool install --global dotnet-ef
    ```
  - Or update global `dotnet-ef` tool.
    ```
    dotnet tool update --global dotnet-ef
    ```

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

## Upgrading

1. Upgrade `TargetFramework` in **.csproj** file to `net8.0`.
   - Optional: Set `ImplicitUsings` to `enable`.
   - Optional: Set `Nullable` to `enable`.
2. Update the following NuGet packages to `8.0.0` or later:
   - Microsoft.EntityFrameworkCore.Design
   - Microsoft.EntityFrameworkCore.SqlServer
   - EntityFrameworkCore.Scaffolding.Handlebars
3. Remove the `EnableNullableReferenceTypes` option from `services.AddHandlebarsScaffolding` in `ScaffoldingDesignTimeServices.ConfigureDesignTimeServices`.
   - Version 6 or greater relies on [support for nullable reference types in EF Core 6](https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types).
4. Run `dotnet ef dbcontext scaffold` command to regenerate entities.
   - You may retain your customized Handlebars templates.
   - [Many-to-many relationships](https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#many-to-many) will be materialized without the need for an intermediate entity.

## Usage

1. Create a new **.NET 8** class library.

2. Add EF Core SQL Server and Tools NuGet packages.
    - `Microsoft.EntityFrameworkCore.SqlServer`
    - `Microsoft.EntityFrameworkCore.Design`

    ```
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Design
    ```

3. Add the **EntityFrameworkCore.Scaffolding.Handlebars** NuGet package:
    - `EntityFrameworkCore.Scaffolding.Handlebars`

    ```
    dotnet add package EntityFrameworkCore.Scaffolding.Handlebars
    ```

4. Remove Class1.cs and add a **ScaffoldingDesignTimeServices** class.
    - Implement `IDesignTimeServices` by adding a `ConfigureDesignTimeServices` method
      that calls `services.AddHandlebarsScaffolding`.
    - You can optionally pass a `ReverseEngineerOptions` enum to indicate if you wish 
      to generate only entity types, only a DbContext class, or both (which is the default).

    ```csharp
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddHandlebarsScaffolding();
        }
    }
    ```

5. Open a command prompt at the project level and use the `dotnet ef` tool to reverse engineer a context and models from an existing database.
    - Get help on _dotnet-ef-dbcontext-scaffold_ at the command line: `dotnet ef dbcontext scaffold -h`
    - Execute the following command to reverse engineer classes from the NorthwindSlim database:

    ```
    dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
    ```

    - You should see context and/or entity classes appear in the _Models_ folder of the project.
    - You will also see a **CodeTemplates** folder appear containing Handlebars templates for customizing generation of context and entity type classes.
    - Add `-d` to the command to use data annotations. You will need to add the **System.ComponentModel.Annotations** package to a .NET Standard library containing linked entity classes.

6. You may _edit_ any of the template files which appear under the **CodeTemplates** folder.
    - For now you can just add some comments, but you may wish to customize the templates in other ways, for example, by inheriting entities from a base class or implementing
    specific interfaces.
    - When you run the _dotnet-ef-dbcontext-scaffold_ command again, you will see your updated reflected in the generated classes.

## Nullable Reference Types

Take advantage of C# nullable reference types by enabling them in your .csproj file. (This is by default in .NET 6 or greater.)

```xml
<PropertyGroup>
  <TargetFramework>net6.0</TargetFramework>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

Non-nullable properties will include the [null forgiving operator](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving).

```csharp
public partial class Product
{
    public string ProductName { get; set; } = null!;
    public decimal? UnitPrice { get; set; }
}
```

## Excluded Tables

You can optionally exclude certain tables from code generation. These may also be qualified by schema name.

```csharp
services.AddHandlebarsScaffolding(options =>
{
    // Exclude some tables
    options.ExcludedTables = new List<string> { "dbo.Territory" };
});
```

## Custom Template Data

You may find it useful to add your own custom template data for use in your Handlebars templates. For example, the model namespace is not included by default in the `DbContext` class import statements. To compensate you may wish to add a `models-namespace` template to the **DbImports.hbs** template file.

```hbs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata; // Comment
using {{models-namespace}};
```

Likewise you may wish to specify the name of a model base class in the same way.

```hbs
public partial class {{class}} : {{base-class}}
{
    {{{> constructor}}}
    {{> properties}}
}
```

You can then set the value of these templates in the `TemplateData` property of `HandlebarsScaffoldingOptions`.

```csharp
services.AddHandlebarsScaffolding(options =>
{
    // Add custom template data
    options.TemplateData = new Dictionary<string, object>
    {
        { "models-namespace", "ScaffoldingSample.Models" },
        { "base-class", "EntityBase" }
    };
});
```

## Schema Folders

You can generate models in different folders by database schema.

```csharp
services.AddHandlebarsScaffolding(options =>
{
    // Put Models into folders by DB Schema
    options.EnableSchemaFolders = true;
});
```

## Embedded Templates

Handlebars templates may be embdedded in a separate .NET Standard project that can be shared among multiple .NET Core scaffolding projects. Simply copy the **CodeTemplates** folder to the .NET Standard project and edit the .csproj file to embed them as a resource in the assembly.

```xml
<ItemGroup>
  <EmbeddedResource Include="CodeTemplates\**\*.hbs" />
</ItemGroup>
```

Then reference the .NET Standard project from the .NET Core projects and specify the templates assembly when adding Handlebars scaffolding in the `ScaffoldingDesignTimeServices` class.

```csharp
public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        // Get templates assembly
        var templatesAssembly = Assembly.Load("TemplatesAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

        // Add Handlebars scaffolding using embedded templates templates
        services.AddHandlebarsScaffolding(options => options.EmbeddedTemplatesAssembly = templatesAssembly);
    }
}
```

## Handlebars Helpers and Transformers

You can register Handlebars helpers in the `ScaffoldingDesignTimeServices` where setup takes place.
- Create a named tuple as shown with `myHelper` below.
- The `context` parameter of the helper method provides model data injected by the Handlebars scaffolding extension.
- Pass the tuple to the `AddHandlebarsHelpers` extension method.
- To use Handlebars helper defined above, add the following to any of the .hbs files within the CodeTemplates folder: `{{my-helper}}`
- You may register as many helpers as you wish.

You can pass transform functions to `AddHandlebarsTransformers` in order to customize generation of entity type definitions, including class names, constructors and properties.

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

            // Enable Nullable reference types
            options.EnableNullableReferenceTypes = true;

            // Put Models into folders by DB Schema
            //options.EnableSchemaFolders = true;

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

        // Add optional Handlebars helpers
        services.AddHandlebarsHelpers(myHelper);

            // Add Handlebars transformer for Country property
            services.AddHandlebarsTransformers(
                propertyTransformer: p =>
                    p.PropertyName == "Country"
                        ? new EntityPropertyInfo("Country?", p.PropertyName, false)
                        : new EntityPropertyInfo(p.PropertyType, p.PropertyName, p.PropertyIsNullable));

            // Add Handlebars transformer for Id property
            //services.AddHandlebarsTransformers2(
            //    propertyTransformer: (e, p) =>
            //        $"{e.Name}Id" == p.PropertyName
            //            ? new EntityPropertyInfo(p.PropertyType, "Id", false)
            //            : new EntityPropertyInfo(p.PropertyType, p.PropertyName, p.PropertyIsNullable));

            // Add optional Handlebars transformers
            //services.AddHandlebarsTransformers2(
            //    entityTypeNameTransformer: n => n + "Foo",
            //    entityFileNameTransformer: n => n + "Foo",
            //    constructorTransformer: (e, p) => new EntityPropertyInfo(p.PropertyType + "Foo", p.PropertyName + "Foo"),
            //    propertyTransformer: (e, p) => new EntityPropertyInfo(p.PropertyType, p.PropertyName + "Foo"),
            //    navPropertyTransformer: (e, p) => new EntityPropertyInfo(p.PropertyType + "Foo", p.PropertyName + "Foo"));
    }

    // Sample Handlebars helper
    void MyHbsHelper(TextWriter writer, Dictionary<string, object> context, object[] parameters)
    {
        writer.Write("// My Handlebars Helper");
    }
}
```

## Extending the OnModelCreating Method

There are times when you might like to modify generated code, for example, by adding a `HasConversion` method to an entity property in the `OnModelCreating` method of the generated class that extends `DbContext`. However, doing so may prove futile because added code would be overwritten the next time you run the `dotnet ef dbcontext scaffold` command.
- Rather than modifying generated code, a better idea would be to extend it by using _partial classes and methods_. To enable this scenario, the generated `DbContext` class is already defined using the `partial` keyword, and it contains a partial `OnModelCreatingPartial` method that is invoked at the end of the `OnModelCreating` method.
- To implement the partial method, simply add a new class to your project with the same name as the generated `DbContext` class, and define it as `partial`. Then add a `OnModelCreatingPartial` method with the same signature as the partial method defined in the generated `DbContext` class.

```csharp
// Place in separate class file (NorthwindSlimContextPartial.cs)
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

## Generating TypeScript Entities

To generate TypeScript entities simply pass `LanguageOptions.TypeScript` to `AddHandlebarsScaffolding`. Since generating a `DbContext` class is strictly a server-side concern, you should also pass `ReverseEngineerOptions.EntitiesOnly` to `AddHandlebarsScaffolding`.

```csharp
public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        // Generate entities only
        var options = ReverseEngineerOptions.EntitiesOnly;

        // Generate TypeScript files
        var language = LanguageOptions.TypeScript;

        // Add Handlebars scaffolding templates
        services.AddHandlebarsScaffolding(options, language);
    }
}
```

## Taking Full Control by Extending Handlebars Generators

> For an example of this approach, see `MyHbsCSharpEntityTypeGenerator` in the [ef-core-community-handlebars](https://github.com/TrackableEntities/ef-core-community-handlebars/blob/master/ScaffoldingHandlebars.Tooling/MyHbsCSharpEntityTypeGenerator.cs) repo.

To take full control of context and entity generation, you can extend `HbsCSharpDbContextGenerator` and `HbsCSharpEntityTypeGenerator`, overriding select virtual methods. Then register your custom generators in `ScaffoldingDesignTimeServices.ConfigureDesignTimeServices`.

For example, you may want to add `property-isprimarykey` to the template data in order to insert some code or a comment.

1. Add a `MyHbsCSharpEntityTypeGenerator` to the **.Tooling** project.
   - Extend `HbsCSharpEntityTypeGenerator`.
   - Override `GenerateProperties`.
   - Copy code from the base `GenerateProperties` method.
   - Add code that inserts `property-isprimarykey` into the template data.
   ```csharp
   protected override void GenerateProperties(IEntityType entityType)
   {
      var properties = new List<Dictionary<string, object>>();
      foreach (var property in entityType.GetProperties().OrderBy(p => p.GetColumnOrdinal()))
      {
         // Code elided for clarity
         properties.Add(new Dictionary<string, object>
         {
               { "property-type", propertyType },
               { "property-name", property.Name },
               { "property-annotations",  PropertyAnnotationsData },
               { "property-comment", property.GetComment() },
               { "property-isnullable", property.IsNullable },
               { "nullable-reference-types", _options?.Value?.EnableNullableReferenceTypes == true },

               // Add new item to template data
               { "property-isprimarykey", property.IsPrimaryKey() }
         });
      }

      var transformedProperties = EntityTypeTransformationService.TransformProperties(properties);

      // Add to transformed properties
      for (int i = 0; i < transformedProperties.Count ; i++)
      {
         transformedProperties[i].Add("property-isprimarykey", properties[i]["property-isprimarykey"]);
      }

      TemplateData.Add("properties", transformedProperties);
   }
   ```
2. Register `MyHbsCSharpEntityTypeGenerator` in `ScaffoldingDesignTimeServices.ConfigureDesignTimeServices`.
   ```csharp
   services.AddSingleton<ICSharpEntityTypeGenerator, MyHbsCSharpEntityTypeGenerator>();
   ```
3. Update **CSharpEntityType/Partials/Properties.hbs** to add `property-isprimarykey`.
   ```handlebars
   {{#if property-isprimarykey}} // Primary Key{{/if}}
   ```
4. Run the `dotnet ef dbcontext scaffold` command from above.
