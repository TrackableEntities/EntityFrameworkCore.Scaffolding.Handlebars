using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Scaffolding.Handlebars.Tests.Models;
using Xunit;

namespace Scaffolding.Handlebars.Tests
{
    public class IModelExtensionsTests
    {
        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_Table_Name_Only()
        {
            var builder = new ModelBuilder(new ConventionSet());
            builder.Entity<Category>()
                .ToTable("Category", "dbo");

            builder.Entity<Product>()
                .ToTable("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> {"Product"}
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);
            Assert.Contains(entityTypes, e => e.ClrType.Name == "Category");
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Product");
        }

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_Table_Name_and_Schema()
        {
            var builder = new ModelBuilder(new ConventionSet());
            builder.Entity<Category>()
                .ToTable("Category", "dbo");

            builder.Entity<Product>()
                .ToTable("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "dbo.Category", "dbo.Product" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Category");
            Assert.Contains(entityTypes, e => e.ClrType.Name == "Product");
        }

        [Fact]
        public void IModel_Extensions_Should_Not_Filter_If_No_Excluded_Tables()
        {
            var builder = new ModelBuilder(new ConventionSet());
            builder.Entity<Category>()
                .ToTable("Category", "dbo");

            builder.Entity<Product>()
                .ToTable("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = null
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);
            Assert.Contains(entityTypes, e => e.ClrType.Name == "Category");
            Assert.Contains(entityTypes, e => e.ClrType.Name == "Product");
        }

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_View_Name_Only()
        {
            var builder = new ModelBuilder(new ConventionSet());
            builder.Entity<Category>()
                .ToView("Category", "dbo");

            builder.Entity<Product>()
                .ToView("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "Product" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);
            Assert.Contains(entityTypes, e => e.ClrType.Name == "Category");
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Product");
        }

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_View_Name_and_Schema()
        {
            var builder = new ModelBuilder(new ConventionSet());
            builder.Entity<Category>()
                .ToView("Category", "dbo");

            builder.Entity<Product>()
                .ToView("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "dbo.Category", "dbo.Product" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Category");
            Assert.Contains(entityTypes, e => e.ClrType.Name == "Product");
        }
    }
}
