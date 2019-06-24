// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
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
                .AddSingleton<ITemplateLanguageService, CSharpTemplateLanguageService>()
                .AddSingleton<IEntityTypeTemplateService, HbsEntityTypeTemplateService>()
                .AddSingleton<IReverseEngineerScaffolder, HbsReverseEngineerScaffolder>()
                .AddSingleton<IEntityTypeTransformationService, HbsEntityTypeTransformationService>()
                .AddSingleton<IHbsHelperService>(provider => new HbsHelperService(
                    new Dictionary<string, Action<TextWriter, Dictionary<string, object>, object[]>>
                    {
                        {Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                    }))
               .BuildServiceProvider()
               .GetRequiredService<IReverseEngineerScaffolder>();

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
