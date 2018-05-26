﻿using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

/*
You may customize generated classes by modifying the Handlebars templates in the CodeTemplates folder.
Then run a 'dotnet ef dbcontext scaffold' command to reverse engineer classes from an existing database.
For example:
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f
*/

namespace ScaffoldingSample
{
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
}
