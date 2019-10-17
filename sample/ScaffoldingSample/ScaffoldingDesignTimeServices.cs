using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            // Register Handlebars helper
            var myHelper = (helperName: "my-helper", helperFunction: (Action<TextWriter, Dictionary<string, object>, object[]>) MyHbsHelper);

            // Register Handlebars block helper
            var ifCondHelper = (helperName: "ifCond", helperFunction: (Action<TextWriter, HelperOptions, Dictionary<string, object>, object[]>)MyHbsBlockHelper);

            // Add Handlebars scaffolding templates
            services.AddHandlebarsScaffolding(options =>
            {
                options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;
            });

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
}
