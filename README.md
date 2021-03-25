# Entity Framework Core Scaffolding with Handlebars

_Scaffold EF Core models using Handlebars templates._

- Uses [Handlebars.NET](https://github.com/rexm/Handlebars.Net) to compile [Handlebars](http://handlebarsjs.com) templates when generating models with the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) scaffolding tools.
 
## EF Core Community Standup

View the [EF Core Community Standup](https://youtu.be/6Ux7EpgiWXE) episode featuring this framework for scaffolding entities with Handlebars templates. The demos for the episode can be found on this GitHub [repo](https://github.com/TrackableEntities/ef-core-community-handlebars).

## Contributing

Before creating a pull request, please refer to the [Contributing Guidelines](https://github.com/TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars/blob/master/.github/CONTRIBUTING.md).

## Prerequisites

- [Visual Studio 2019](https://www.visualstudio.com/downloads/) 16.8 Preview 4.0 or greater.
- .[NET Core 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) RC2 or greater.
- [EF Core CLI 5.0](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-ef-core-5-rc2/) or greater.
  - Install global `dotnet-ef` tool.
    ```
    dotnet tool install --global dotnet-ef
    ```
  - Update global `dotnet-ef` tool.
    ```
    dotnet tool update --global dotnet-ef
    ```

## Database Setup

1. Use SQL Server Management Studio to connect to SQL Server
    - The easiest is to use **LocalDb**, which is installed with Visual Studio.  
    Connect to: `(localdb)\MsSqlLocalDb`.
    - Create a new database named **NorthwindSlim**.
    - Download the `NorthwindSlim.sql` file from <https://github.com/TrackableEntities/northwind-slim>.
    - Unzip **NorthwindSlim.sql** and run the script to create tables and populate them with data.

## Usage

1. Create a new **.NET Core** class library.
    - If necessary, edit the csproj file to update the **TargetFramework** to 3.1 or 5.0

    > **Note**: Using the EF Core toolchain with a _.NET Standard_ class library is currently not supported. Instead, you can add a .NET Standard class library to the same solution as the .NET Core library, then add existing items and select **Add As Link** to include entity classes.

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

Take advantage of C# nullable reference types by enabling them in your .csproj file.

```xml
<PropertyGroup>
  <TargetFramework>netcoreapp3.1</TargetFramework>
  <LangVersion>8.0</LangVersion>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

Then enable nullable reference types for Handlebars scaffolding.

```csharp
services.AddHandlebarsScaffolding(options =>
{
    options.EnableNullableReferenceTypes = true;
});
```

Non-nullable properties will include the [null forgiving operator](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving).

```csharp
public partial class Product
{
    public string ProductName { get; set; } = default!;
    public decimal? UnitPrice { get; set; }
}
```

## Excluded Tables

You can optionally exclude certain tables from code generation. These may also be qualified by schema name.

```csharp
services.AddHandlebarsScaffolding(options =>
{
    // Exclude some tables
    options.ExcludedTables = new List<string> { "Territory", "dbo.EmployeeTerritories" };
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
            propertyTransformer: e =>
                e.PropertyName == "Country"
                    ? new EntityPropertyInfo("Country", e.PropertyName, false)
                    : new EntityPropertyInfo(e.PropertyType, e.PropertyName, e.PropertyIsNullable));

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

## Adding additional Template Driectories and customizing Generation for other languages

To generate Custom entities/context files and use your own custom named template directories, just create your own LanguageOptions class 

```csharp
public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        // Generate both context and entities
        options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;

        // Generate My files
        var language = new LanguageOptions
        {
            FileExtension = ".my",
            DbContextDirectory = Constants.CodeTemplatesDirectory + "/MyDbContext",
            DbContextPartialsDirectory = Constants.CodeTemplatesDirectory + "/MyDbContext/Partials",
            EntityTypeDirectory = Constants.CodeTemplatesDirectory + "/MyEntityType",
            EntityTypePartialsDirectory = Constants.CodeTemplatesDirectory + "/MyEntityType/Partials",
            AddTripleSlashToComments = false,
            PropertyNameConversion =  LanguageOptions.TypeScript.PropertyNameConversion,
            TypeNameConversion =  LanguageOptions.TypeScript.TypeNameConversion,
            EntityTypeImportListGenerator = LanguageOptions.TypeScript.EntityTypeImportListGenerator
        };

        // Add Handlebars scaffolding templates
        services.AddHandlebarsScaffolding(options, language);
    }
}
```
