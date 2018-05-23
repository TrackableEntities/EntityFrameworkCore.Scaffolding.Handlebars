// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Scaffolding.Handlebars.Tests.Fakes;
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
            //var cSharpUtilities = new CSharpUtilities();
            //var fileService = new InMemoryTemplateFileService();
            //var dbContextTemplateService = new HbsDbContextTemplateService(fileService);
            //var entityTypeTemplateService = new HbsEntityTypeTemplateService(fileService);

            //var reverseEngineer_old = new ReverseEngineerScaffolder(
            //    new FakeDatabaseModelFactory(),
            //    new FakeScaffoldingModelFactory(new TestOperationReporter()),
            //    new HbsCSharpScaffoldingGenerator(
            //        fileService,
            //        dbContextTemplateService,
            //        entityTypeTemplateService,
            //        new HbsCSharpDbContextGenerator(
            //            new FakeScaffoldingProviderCodeGenerator(),
            //            new FakeAnnotationCodeGenerator(),
            //            cSharpUtilities,
            //            new HbsDbContextTemplateService(fileService)),
            //        new HbsCSharpEntityTypeGenerator(
            //            cSharpUtilities,
            //            new HbsEntityTypeTemplateService(fileService))),
            //            cSharpUtilities);

            var reverseEngineer = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<IRelationalTypeMappingSource, TestRelationalTypeMappingSource>()
                .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
                .AddSingleton<IDatabaseModelFactory, FakeDatabaseModelFactory>()
                .AddSingleton<IScaffoldingModelFactory, FakeScaffoldingModelFactory>()
                .AddSingleton<IProviderConfigurationCodeGenerator, TestProviderCodeGenerator>()
                .AddSingleton<IModelCodeGenerator, HbsCSharpModelGenerator>()
                .AddSingleton<ICSharpDbContextGenerator, HbsCSharpDbContextGenerator>()
                .AddSingleton<ICSharpEntityTypeGenerator, HbsCSharpEntityTypeGenerator>()
                .AddSingleton<IDbContextTemplateService, HbsDbContextTemplateService>()
                .AddSingleton<ITemplateFileService, InMemoryTemplateFileService>()
                .AddSingleton<IEntityTypeTemplateService, HbsEntityTypeTemplateService>()
               .BuildServiceProvider()
                .GetRequiredService<IReverseEngineerScaffolder>();

            //Assert.Equal(
            //    DesignStrings.ContextClassNotValidCSharpIdentifier(contextName),
            //    Assert.Throws<ArgumentException>(
            //            () => reverseEngineer.Generate(
            //                connectionString: "connectionstring",
            //                tables: Enumerable.Empty<string>(),
            //                schemas: Enumerable.Empty<string>(),
            //                projectPath: "FakeProjectPath",
            //                outputPath: null,
            //                rootNamespace: "FakeNamespace",
            //                contextName: contextName,
            //                useDataAnnotations: false,
            //                overwriteFiles: false,
            //                useDatabaseNames: false))
            //        .Message);

            Assert.Equal(
                DesignStrings.ContextClassNotValidCSharpIdentifier(contextName),
                Assert.Throws<ArgumentException>(
                        () => reverseEngineer.ScaffoldModel(
                            connectionString: "connectionstring",
                            tables: Enumerable.Empty<string>(),
                            schemas: Enumerable.Empty<string>(),
                            @namespace: "FakeNamespace",
                            language: "",
                            contextDir: null,
                            contextName: contextName,
                            modelOptions: new ModelReverseEngineerOptions(),
                            codeOptions: new ModelCodeGenerationOptions()))
                    .Message);
        }
    }
}
