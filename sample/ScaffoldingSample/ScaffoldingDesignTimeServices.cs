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
            // Generate both context and entities
            var options = ReverseEngineerOptions.DbContextAndEntities;

            // Register Handlebars helper
            var myHelper = (helperName: "my-helper", helperFunction: (Action<TextWriter, Dictionary<string, object>, object[]>) MyHbsHelper);

            var ifCondHelper = (helperName: "ifCond", helperFunction: (Action<TextWriter, HelperOptions, Dictionary<string, object>, object[]>)MyHbsBlockHelper);


            // Add Handlebars scaffolding templates
            services.AddHandlebarsScaffolding(options);

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
            if (args.Length != 3)
            {
                writer.Write("ifCond:Wrong number of arguments");
                return;
            }
            if (args[0] == null || args[0].GetType().Name == "UndefinedBindingResult")
            {
                writer.Write("ifCond:args[0] undefined");
                return;
            }
            if (args[1] == null || args[1].GetType().Name == "UndefinedBindingResult")
            {
                writer.Write("ifCond:args[1] undefined");
                return;
            }
            if (args[2] == null || args[2].GetType().Name == "UndefinedBindingResult")
            {
                writer.Write("ifCond:args[2] undefined");
                return;
            }
            if (args[0].GetType().Name == "String")
            {
                var val1 = args[0].ToString();
                var val2 = args[2].ToString();

                switch (args[1].ToString())
                {
                    case ">":
                        if (val1.Length > val2.Length)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                    case "=":
                    case "==":
                        if (val1 == val2)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                    case "<":
                        if (val1.Length < val2.Length)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                    case "!=":
                    case "<>":
                        if (val1 != val2)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                }
            }
            else
            {
                var val1 = float.Parse(args[0].ToString());
                var val2 = float.Parse(args[2].ToString());

                switch (args[1].ToString())
                {
                    case ">":
                        if (val1 > val2)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                    case "=":
                    case "==":
                        if (val1 == val2)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                    case "<":
                        if (val1 < val2)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                    case "!=":
                    case "<>":
                        if (val1 != val2)
                        {
                            options.Template(writer, (object)context);
                        }
                        else
                        {
                            options.Inverse(writer, (object)context);
                        }
                        break;
                }
            }
        }
    }
}
