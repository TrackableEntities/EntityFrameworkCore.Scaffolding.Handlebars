// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Scaffolding.Handlebars.Tests.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class ReverseEngineerScaffolder : IReverseEngineerScaffolder
    {
        private readonly IDatabaseModelFactory _databaseModelFactory;
        private readonly IScaffoldingModelFactory _factory;
        private readonly ICSharpUtilities _cSharpUtilities;
        private static readonly char[] _directorySeparatorChars = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        private const string DbContextSuffix = "Context";
        private const string DefaultDbContextName = "Model" + DbContextSuffix;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public ReverseEngineerScaffolder(
            IDatabaseModelFactory databaseModelFactory,
            IScaffoldingModelFactory scaffoldingModelFactory,
            IScaffoldingCodeGenerator scaffoldingCodeGenerator,
            ICSharpUtilities cSharpUtilities)
        {
            _databaseModelFactory = databaseModelFactory ?? throw new ArgumentNullException(nameof(databaseModelFactory));
            _factory = scaffoldingModelFactory ?? throw new ArgumentNullException(nameof(scaffoldingModelFactory));
            ScaffoldingCodeGenerator = scaffoldingCodeGenerator ?? throw new ArgumentNullException(nameof(scaffoldingCodeGenerator));
            _cSharpUtilities = cSharpUtilities ?? throw new ArgumentNullException(nameof(cSharpUtilities));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        private IScaffoldingCodeGenerator ScaffoldingCodeGenerator { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ReverseEngineerFiles Generate(
            string connectionString,
            IEnumerable<string> tables,
            IEnumerable<string> schemas,
            string projectPath,
            string outputPath,
            string rootNamespace,
            string contextName,
            bool useDataAnnotations,
            bool overwriteFiles,
            bool useDatabaseNames)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            if (tables == null) throw new ArgumentNullException(nameof(tables));
            if (schemas == null) throw new ArgumentNullException(nameof(schemas));
            if (projectPath == null) throw new ArgumentNullException(nameof(projectPath));
            if (rootNamespace == null) throw new ArgumentNullException(nameof(rootNamespace));

            if (!string.IsNullOrWhiteSpace(contextName)
                && (!_cSharpUtilities.IsValidIdentifier(contextName)
                    || _cSharpUtilities.IsCSharpKeyword(contextName)))
            {
                throw new ArgumentException(
                    DesignStrings.ContextClassNotValidCSharpIdentifier(contextName));
            }

            var databaseModel = _databaseModelFactory.Create(connectionString, tables, schemas);
            //var model = _factory.Create(connectionString, tables, schemas, useDatabaseNames);
            var model = _factory.Create(databaseModel, useDatabaseNames);

            if (model == null)
            {
                throw new InvalidOperationException(
                    DesignStrings.ProviderReturnedNullModel(
                        _factory.GetType().ShortDisplayName()));
            }

            outputPath = string.IsNullOrWhiteSpace(outputPath) ? null : outputPath;
            var subNamespace = SubnamespaceFromOutputPath(projectPath, outputPath);

            var @namespace = rootNamespace;

            if (!string.IsNullOrEmpty(subNamespace))
            {
                @namespace += "." + subNamespace;
            }

            if (string.IsNullOrEmpty(contextName))
            {
                contextName = DefaultDbContextName;

                var annotatedName = model.Scaffolding().DatabaseName;
                if (!string.IsNullOrEmpty(annotatedName))
                {
                    contextName = _cSharpUtilities.GenerateCSharpIdentifier(
                        annotatedName + DbContextSuffix,
                        existingIdentifiers: null,
                        singularizePluralizer: null);
                }
            }

            CheckOutputFiles(outputPath ?? projectPath, contextName, model, overwriteFiles);

            return ScaffoldingCodeGenerator.WriteCode(model, outputPath ?? projectPath, @namespace, contextName, connectionString, useDataAnnotations);
        }

        // if outputDir is a subfolder of projectDir, then use each subfolder as a subnamespace
        // --output-dir $(projectFolder)/A/B/C
        // => "namespace $(rootnamespace).A.B.C"
        private string SubnamespaceFromOutputPath(string projectDir, string outputDir)
        {
            if (outputDir == null
                || !outputDir.StartsWith(projectDir, StringComparison.Ordinal))
            {
                return null;
            }

            var subPath = outputDir.Substring(projectDir.Length);

            return !string.IsNullOrWhiteSpace(subPath)
                ? string.Join(".", subPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                : null;
        }

        private void CheckOutputFiles(
            string outputPath,
            string dbContextClassName,
            IModel metadataModel,
            bool overwriteFiles)
        {
            if (dbContextClassName == null) throw new ArgumentNullException(nameof(dbContextClassName));
            if (metadataModel == null) throw new ArgumentNullException(nameof(metadataModel));
            if (metadataModel == null) throw new ArgumentNullException(nameof(metadataModel));

            var readOnlyFiles = ScaffoldingCodeGenerator.GetReadOnlyFilePaths(
                outputPath, dbContextClassName, metadataModel.GetEntityTypes());

            if (readOnlyFiles.Count > 0)
            {
                throw new InvalidOperationException(
                    DesignStrings.ReadOnlyFiles(
                        outputPath,
                        string.Join(
                            CultureInfo.CurrentCulture.TextInfo.ListSeparator, readOnlyFiles)));
            }

            if (!overwriteFiles)
            {
                var existingFiles = ScaffoldingCodeGenerator.GetExistingFilePaths(
                    outputPath, dbContextClassName, metadataModel.GetEntityTypes());
                if (existingFiles.Count > 0)
                {
                    throw new InvalidOperationException(
                        DesignStrings.ExistingFiles(
                            outputPath,
                            string.Join(
                                CultureInfo.CurrentCulture.TextInfo.ListSeparator, existingFiles)));
                }
            }
        }
    }
}
