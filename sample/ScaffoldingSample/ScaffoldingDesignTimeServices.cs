﻿using System;
using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

/*
You may customize generated classes by modifying the Handlebars templates in the CodeTemplates folder.
Then run a 'dotnet ef dbcontext scaffold' command to reverse engineer classes from an existing database.
For example:
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c NorthwindSlimContext -f --context-dir Contexts
*/

namespace ScaffoldingSample
{
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            // Uncomment to launch JIT debugger and hit breakpoints
            //Debugger.Launch();

            // Add Handlebars scaffolding templates
            services.AddHandlebarsScaffolding(options =>
            {
                // Generate both context and entities
                options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;

                // Put Models into folders by DB Schema
                options.EnableSchemaFolders = true;

                // Exclude some tables
                options.ExcludedTables = new List<string> { "Territory", "EmployeeTerritories" };

                // Add custom template data
                options.TemplateData = new Dictionary<string, object>
                {
                    { "models-namespace", "ScaffoldingSample.Models" },
                    { "base-class", "EntityBase" }
                };

                // Disable comments generation
                options.GenerateComments = false;
            });

            // Register Handlebars helper
            var myHelper = (helperName: "my-helper", helperFunction: (Action<EncodedTextWriter, Context, Arguments>)MyHbsHelper);

            // Register Handlebars block helper
            var ifCondHelper = (helperName: "ifCond", helperFunction: (Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>)MyHbsBlockHelper);

            // Add optional Handlebars helpers
            services.AddHandlebarsHelpers(myHelper);
            services.AddHandlebarsBlockHelpers(ifCondHelper);

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
        void MyHbsHelper(EncodedTextWriter writer, Context context, Arguments parameters)
        {
            writer.Write("// My Handlebars Helper");
        }

        // Sample Handlebars block helper
        void MyHbsBlockHelper(EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments args)
        {
            var val0str = args[0]?.ToString();
            var val1str = args[1]?.ToString();
            var val2str = args[2]?.ToString();
            var val1 = float.Parse(val0str ?? "0");
            var val2 = float.Parse(val2str ?? "0");

            switch (val1str)
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
}
