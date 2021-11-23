// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System;
using System.Collections.Generic;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scaffolding.Handlebars.Tests.Fakes;
using Scaffolding.Handlebars.Tests.Helpers;
using Xunit;
using Constants = EntityFrameworkCore.Scaffolding.Handlebars.Helpers.Constants;

namespace Scaffolding.Handlebars.Tests
{
    public class TestOperationReporter : IOperationReporter
    {
        private readonly List<(LogLevel, string)> _messages = new();

        public IReadOnlyList<(LogLevel Level, string Message)> Messages
            => _messages;

        public void Clear()
            => _messages.Clear();

        public void WriteInformation(string message)
            => _messages.Add((LogLevel.Information, message));

        public void WriteVerbose(string message)
            => _messages.Add((LogLevel.Debug, message));

        public void WriteWarning(string message)
            => _messages.Add((LogLevel.Warning, message));

        public void WriteError(string message)
            => _messages.Add((LogLevel.Error, message));
    }

    public class ReverseEngineeringConfigurationTests
    {
        [Theory]
        [InlineData("Invalid!CSharp*Class&Name")]
        [InlineData("1CSharpClassNameCannotStartWithNumber")]
        [InlineData("volatile")]
        public void ValidateContextNameInReverseEngineerGenerator(string contextName)
        {
            var reverseEngineer = new DesignTimeServicesBuilder(
                typeof(ReverseEngineeringConfigurationTests).Assembly,
                typeof(ReverseEngineeringConfigurationTests).Assembly,
                new TestOperationReporter(),
                new string[0])
                .CreateServiceCollection("Microsoft.EntityFrameworkCore.SqlServer")
                .AddSingleton<LoggingDefinitions, TestRelationalLoggingDefinitions>()
                .AddSingleton<IRelationalTypeMappingSource, TestRelationalTypeMappingSource>()
                .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
                .AddSingleton<IDatabaseModelFactory, FakeDatabaseModelFactory>()
                .AddSingleton<IProviderConfigurationCodeGenerator, TestProviderCodeGenerator>()
                .AddSingleton<IScaffoldingModelFactory, FakeScaffoldingModelFactory>()
                .AddSingleton<IModelCodeGenerator, HbsCSharpModelGenerator>()
                .AddSingleton<ICSharpDbContextGenerator, HbsCSharpDbContextGenerator>()
                .AddSingleton<ICSharpEntityTypeGenerator, HbsCSharpEntityTypeGenerator>()
                .AddSingleton<IDbContextTemplateService, HbsDbContextTemplateService>()
                .AddSingleton<ITemplateFileService, InMemoryTemplateFileService>()
                .AddSingleton<ITemplateLanguageService, FakeCSharpTemplateLanguageService>()
                .AddSingleton<IEntityTypeTemplateService, HbsEntityTypeTemplateService>()
                .AddSingleton<IReverseEngineerScaffolder, HbsReverseEngineerScaffolder>()
                .AddSingleton<IEntityTypeTransformationService, HbsEntityTypeTransformationService>()
                .AddSingleton<IContextTransformationService, HbsContextTransformationService>()
                .AddSingleton<IHbsHelperService>(provider => new HbsHelperService(
                    new Dictionary<string, Action<EncodedTextWriter, Context, Arguments>>
                    {
                        {Constants.SpacesHelper, HandlebarsHelpers.SpacesHelper}
                    }))
                .AddSingleton<IHbsBlockHelperService>(provider =>
                    new HbsBlockHelperService(new Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>>()))
                .BuildServiceProvider(validateScopes: true)
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<IReverseEngineerScaffolder>();


            Assert.Equal(DesignStrings.ContextClassNotValidCSharpIdentifier(contextName),
                Assert.Throws<ArgumentException>(
                        () => reverseEngineer.ScaffoldModel(
                            connectionString: "connectionstring",
                            databaseOptions: new DatabaseModelFactoryOptions(),
                            modelOptions: new ModelReverseEngineerOptions(),
                            codeOptions: new ModelCodeGenerationOptions()
                            {
                                ModelNamespace = "FakeNamespace",
                                ContextName = contextName,
                            }))
                    .Message);
        }
    }
}
