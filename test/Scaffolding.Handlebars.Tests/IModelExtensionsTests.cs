// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_TableName_Pattern()
        {
            var builder = new ModelBuilder(new ConventionSet());

            builder.Entity<Category>()
                .ToTable("Category", "dbo");

            builder.Entity<Customer>()
                .ToTable("Customer", "dbo");

            builder.Entity<Product>()
                .ToTable("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "Prod*", "C??tomer" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);

            Assert.Contains(entityTypes, e => e.ClrType.Name == "Category");

            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Product");
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Customer");
        }

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_TableSchema_Pattern()
        {
            var builder = new ModelBuilder(new ConventionSet());

            builder.Entity<Category>()
                .ToTable("Category", "dbo");

            builder.Entity<Customer>()
                .ToTable("Customer", "dbo");

            builder.Entity<Product>()
                .ToTable("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "dbo.*" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);

            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Category");
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Customer");

            Assert.Contains(entityTypes, e => e.ClrType.Name == "Product");
        }

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_ViewName_Pattern()
        {
            var builder = new ModelBuilder(new ConventionSet());

            builder.Entity<Category>()
                .ToView("Category", "dbo");

            builder.Entity<Customer>()
                .ToView("Customer", "dbo");

            builder.Entity<Product>()
                .ToView("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "Prod*", "C??tomer" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);

            Assert.Contains(entityTypes, e => e.ClrType.Name == "Category");

            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Product");
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Customer");
        }

        [Fact]
        public void IModel_Extensions_Should_Filter_EntityTypes_From_Options_By_ViewSchema_Pattern()
        {
            var builder = new ModelBuilder(new ConventionSet());

            builder.Entity<Category>()
                .ToView("Category", "dbo");

            builder.Entity<Customer>()
                .ToView("Customer", "dbo");

            builder.Entity<Product>()
                .ToView("Product", "prd");

            var options = new HandlebarsScaffoldingOptions
            {
                ExcludedTables = new List<string> { "dbo.*" }
            };

            var entityTypes = builder.FinalizeModel().GetScaffoldEntityTypes(options);

            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Category");
            Assert.DoesNotContain(entityTypes, e => e.ClrType.Name == "Customer");

            Assert.Contains(entityTypes, e => e.ClrType.Name == "Product");
        }
    }
}
