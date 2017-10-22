// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Scaffolding.Handlebars.Tests.Fakes;
using Scaffolding.Handlebars.Tests.Internal;
using Xunit;

namespace Scaffolding.Handlebars.Tests
{
    public class ReverseEngineeringConfigurationTests
    {
        [Theory]
        [InlineData("Invalid!CSharp*Class&Name")]
        [InlineData("1CSharpClassNameCannotStartWithNumber")]
        [InlineData("volatile")]
        public void ValidateContextNameInReverseEngineerGenerator(string contextName)
        {
            var cSharpUtilities = new CSharpUtilities();
            var fileService = new InMemoryTemplateFileService();
            var dbContextTemplateService = new HbsDbContextTemplateService(fileService);
            var entityTypeTemplateService = new HbsEntityTypeTemplateService(fileService);

            var reverseEngineer = new ReverseEngineerScaffolder(
                new FakeDatabaseModelFactory(),
                new FakeScaffoldingModelFactory(new TestOperationReporter()),
                new HbsCSharpScaffoldingGenerator(
                    fileService,
                    dbContextTemplateService,
                    entityTypeTemplateService,
                    new HbsCSharpDbContextGenerator(
                        new FakeScaffoldingProviderCodeGenerator(),
                        new FakeAnnotationCodeGenerator(),
                        cSharpUtilities,
                        new HbsDbContextTemplateService(fileService)),
                    new HbsCSharpEntityTypeGenerator(
                        cSharpUtilities,
                        new HbsEntityTypeTemplateService(fileService))),
                        cSharpUtilities);

            Assert.Equal(
                DesignStrings.ContextClassNotValidCSharpIdentifier(contextName),
                Assert.Throws<ArgumentException>(
                        () => reverseEngineer.Generate(
                            connectionString: "connectionstring",
                            tables: Enumerable.Empty<string>(),
                            schemas: Enumerable.Empty<string>(),
                            projectPath: "FakeProjectPath",
                            outputPath: null,
                            rootNamespace: "FakeNamespace",
                            contextName: contextName,
                            useDataAnnotations: false,
                            overwriteFiles: false,
                            useDatabaseNames: false))
                    .Message);
        }
    }
}
