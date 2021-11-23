// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Scaffolding for persisting generated DbContext and entity type classes using Handlebars templates.
    /// </summary>
    public class HbsReverseEngineerScaffolder : ReverseEngineerScaffolder
    {
        /// <summary>
        /// Constructor for the HbsCSharpModelGenerator.
        /// </summary>
        /// <param name="databaseModelFactory">Service to reverse engineer a database into a database model.</param>
        /// <param name="scaffoldingModelFactory">Factory to create a scaffolding model.</param>
        /// <param name="modelCodeGeneratorSelector">Selects a model code generator service for a given programming language.</param>
        /// <param name="cSharpUtilities">C# utilities.</param>
        /// <param name="cSharpHelper">C# helper.</param>
        /// <param name="connectionStringResolver">Connection string resolver.</param>
        /// <param name="reporter">Operation reporter.</param>
        public HbsReverseEngineerScaffolder(
            IDatabaseModelFactory databaseModelFactory,
            IScaffoldingModelFactory scaffoldingModelFactory,
            IModelCodeGeneratorSelector modelCodeGeneratorSelector,
            ICSharpUtilities cSharpUtilities,
            ICSharpHelper cSharpHelper,
            IDesignTimeConnectionStringResolver connectionStringResolver,
            IOperationReporter reporter)
            : base(databaseModelFactory, scaffoldingModelFactory, modelCodeGeneratorSelector, cSharpUtilities, cSharpHelper, connectionStringResolver, reporter)
        {
            Check.NotNull(databaseModelFactory, nameof(databaseModelFactory));
            Check.NotNull(scaffoldingModelFactory, nameof(scaffoldingModelFactory));
            Check.NotNull(modelCodeGeneratorSelector, nameof(modelCodeGeneratorSelector));
            Check.NotNull(cSharpUtilities, nameof(cSharpUtilities));
            Check.NotNull(cSharpHelper, nameof(cSharpHelper));
            Check.NotNull(connectionStringResolver, nameof(connectionStringResolver));
            Check.NotNull(reporter, nameof(reporter));
        }

        /// <summary>
        /// Persist generated DbContext and entity type classes. 
        /// </summary>
        /// <param name="scaffoldedModel">Represents a scaffolded model.</param>
        /// <param name="outputDir">Output directory.</param>
        /// <param name="overwriteFiles">True to overwrite existing files.</param>
        /// <returns></returns>
        public override SavedModelFiles Save(ScaffoldedModel scaffoldedModel, string outputDir, bool overwriteFiles)
        {
            CheckOutputFiles(scaffoldedModel, outputDir, overwriteFiles);

            Directory.CreateDirectory(outputDir);

            var contextPath = string.Empty;
            if (scaffoldedModel.ContextFile != null
                && !string.IsNullOrWhiteSpace(scaffoldedModel.ContextFile.Path))
            {
                contextPath = Path.GetFullPath(Path.Combine(outputDir, scaffoldedModel.ContextFile.Path));
                Directory.CreateDirectory(Path.GetDirectoryName(contextPath));
                File.WriteAllText(contextPath, scaffoldedModel.ContextFile.Code, Encoding.UTF8);
            }

            var additionalFiles = new List<string>();
            foreach (var entityTypeFile in scaffoldedModel.AdditionalFiles)
            {
                var additionalFilePath = Path.Combine(outputDir, entityTypeFile.Path);
                Directory.CreateDirectory(Path.GetDirectoryName(additionalFilePath));
                File.WriteAllText(additionalFilePath, entityTypeFile.Code, Encoding.UTF8);
                additionalFiles.Add(additionalFilePath);
            }

            return new SavedModelFiles(contextPath, additionalFiles);
        }

        private static void CheckOutputFiles(
            ScaffoldedModel scaffoldedModel,
            string outputDir,
            bool overwriteFiles)
        {
            var paths = scaffoldedModel.AdditionalFiles.Select(f => f.Path).ToList();
            if (scaffoldedModel.ContextFile != null
                && !string.IsNullOrWhiteSpace(scaffoldedModel.ContextFile.Path))
            paths.Insert(0, scaffoldedModel.ContextFile.Path);

            var existingFiles = new List<string>();
            var readOnlyFiles = new List<string>();
            foreach (var path in paths)
            {
                var fullPath = Path.Combine(outputDir, path);

                if (File.Exists(fullPath))
                {
                    existingFiles.Add(path);

                    if (File.GetAttributes(fullPath).HasFlag(FileAttributes.ReadOnly))
                    {
                        readOnlyFiles.Add(path);
                    }
                }
            }

            if (!overwriteFiles && existingFiles.Count != 0)
            {
                throw new OperationException(
                    DesignStrings.ExistingFiles(
                        outputDir,
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, existingFiles)));
            }
            if (readOnlyFiles.Count != 0)
            {
                throw new OperationException(
                    DesignStrings.ReadOnlyFiles(
                        outputDir,
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, readOnlyFiles)));
            }
        }
    }
}