// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Scaffolding.Handlebars.Tests.Internal
{
    public class FakeScaffoldingModelFactory : RelationalScaffoldingModelFactory
    {
        public override IModel Create(DatabaseModel databaseModel, bool useDatabaseNames = false)
        {
            foreach (var sequence in databaseModel.Sequences)
            {
                sequence.Database = databaseModel;
            }

            foreach (var table in databaseModel.Tables)
            {
                table.Database = databaseModel;

                foreach (var column in table.Columns)
                {
                    column.Table = table;
                }

                if (table.PrimaryKey != null)
                {
                    table.PrimaryKey.Table = table;
                }

                foreach (var index in table.Indexes)
                {
                    index.Table = table;
                }

                foreach (var uniqueConstraints in table.UniqueConstraints)
                {
                    uniqueConstraints.Table = table;
                }

                foreach (var foreignKey in table.ForeignKeys)
                {
                    foreignKey.Table = table;
                }
            }

            return base.Create(databaseModel, useDatabaseNames);
        }

        public FakeScaffoldingModelFactory(
            IOperationReporter reporter)
            : this(reporter, new NullPluralizer())
        {
        }

        public FakeScaffoldingModelFactory(
            IOperationReporter reporter,
            IPluralizer pluralizer)
            : base(
                reporter,
                new CandidateNamingService(),
                pluralizer,
                new CSharpUtilities(),
                new ScaffoldingTypeMapper(new SqlServerTypeMapper(new RelationalTypeMapperDependencies())))
        {
        }
    }

    public class FakeDatabaseModelFactory : IDatabaseModelFactory
    {
        public virtual DatabaseModel Create(string connectionString, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseModel Create(DbConnection connectio, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            throw new NotImplementedException();
        }
    }

    public class FakePluralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            return name.EndsWith("s")
                ? name
                : name + "s";
        }

        public string Singularize(string name)
        {
            return name.EndsWith("s")
                ? name.Substring(0, name.Length - 1)
                : name;
        }
    }
}