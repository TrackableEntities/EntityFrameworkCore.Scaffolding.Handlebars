# Entity Framework Core Scaffolding with Handlebars

Scaffold EF Core models using Handlebars templates.

- Uses [Handlebars.NET](https://github.com/rexm/Handlebars.Net) to compile [Handlebars](http://handlebarsjs.com) templates when generating models with the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) scaffolding tools.
 
## Prerequisites

- [Visual Studio 2017](https://www.visualstudio.com/downloads/) 15.3 or greater.
- The .[NET Core 2.0 SDK](https://www.microsoft.com/net/download/core)

## Database Setup

1. Use SQL Server Management Studio to connect to SQL Server
    - The easiest is to use **LocalDb**, which is installed with Visual Studio.  
    Connect to: `(localdb)\MsSqlLocalDb`.
    - Create a new database named **NorthwindSlim**.
    - Download the data file from <http://bit.ly/northwindslim>.
    - Unzip **NorthwindSlim.sql** and run the script to create tables and populate them with data.

## Usage

1. Create a new **.NET Core** class library.

    > **Note**: Using the EF Core toolchain with a _.NET Standard_ class library is currently not supported. Instead, you can add a .NET Standard class library to the same solution as the .NET Core library, then add existing items and select **Add As Link** to include entity classes.

2. Add EF Core SQL Server and Tools NuGet packages.  
   Open the Package Manager Console, select the default project and enter:
    - `Install-Package Microsoft.EntityFrameworkCore.SqlServer`
    - `Install-Package Microsoft.EntityFrameworkCore.Tools`

3. Edit the **.csproj** file to add the following section:

    ```xml
    <ItemGroup>
        <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
    </ItemGroup>
    ```

    - Open a command prompt at the project location and run: `dotnet restore`.

4. Add the **EntityFrameworkCore.Scaffolding.Handlebars** NuGet package:
    - `Install-Package EntityFrameworkCore.Scaffolding.Handlebars -Pre`

5. Remove Class1.cs and add a **ScaffoldingDesignTimeServices** class.
    - Implement `IDesignTimeServices` by adding a `ConfigureDesignTimeServices` method
      that calls `services.AddHandlebarsScaffolding`.
    - You can optionally pass a `ReverseEngineerOptions` enum to indicate if you wish 
      to generate only entity types, only a DbContext class, or both (which is the default).

    ```csharp
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            var options = ReverseEngineerOptions.DbContextAndEntities;
            services.AddHandlebarsScaffolding(options);
        }
    }
    ```

5. Open a command prompt and use the **EF .NET CLI** tools to reverse engineer a context and models from an existing database.
    - Get help on _dotnet-ef-dbcontext-scaffold_ at the command line: `dotnet ef dbcontext scaffold -h`
    - Execute the following command to reverse engineer classes from the NorthwindSlim database:

    ```
    dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f
    ```

    - You should see context and/or entity classes appear in the _Models_ folder of the project.
    - You will also see a **CodeTemplates** folder appear containing Handlebars templates for customizing generation of context and entity type classes.
    - Add `-d` to the command to use data annotations. You will need to add the **System.ComponentModel.Annotations** package to a .NET Standard library containing linked entity classes.

6. You may _edit_ any of the template files which appear under the **CodeTemplates** folder.
    - For now you can just add some comments, but you may wish to customize the templates in other ways, for example, by inheriting entities from a base class or implementing
    specific interfaces.
    - When you run the _dotnet-ef-dbcontext-scaffold_ command again, you will see your updated reflected in the generated classes.

