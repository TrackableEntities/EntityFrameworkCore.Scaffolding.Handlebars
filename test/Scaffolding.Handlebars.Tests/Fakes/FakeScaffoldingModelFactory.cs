using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeScaffoldingModelFactory : RelationalScaffoldingModelFactory
    {
        public FakeScaffoldingModelFactory(
            IOperationReporter reporter,
            ICandidateNamingService candidateNamingService,
            IPluralizer pluralizer,
            ICSharpUtilities cSharpUtilities,
            IScaffoldingTypeMapper scaffoldingTypeMapper,
            IModelRuntimeInitializer modelRuntimeInitializer)
            : base(reporter, candidateNamingService, pluralizer, cSharpUtilities, scaffoldingTypeMapper, modelRuntimeInitializer)
        {
        }

        public override IModel Create(DatabaseModel databaseModel, ModelReverseEngineerOptions options)
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

            return base.Create(databaseModel, options);
        }
    }
}