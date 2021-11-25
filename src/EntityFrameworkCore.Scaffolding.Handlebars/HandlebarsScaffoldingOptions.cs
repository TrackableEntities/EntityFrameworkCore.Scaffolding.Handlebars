// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2019 Tony Sneed.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Options for generating scaffolding with handlebars.
    /// </summary>
    public class HandlebarsScaffoldingOptions
    {
        /// <summary>
        /// Gets or sets which type of scaffolding should be generated.
        /// </summary>
        public ReverseEngineerOptions ReverseEngineerOptions { get; set; } = ReverseEngineerOptions.DbContextAndEntities;

        /// <summary>
        /// Gets or sets language options for generated scaffolding.
        /// </summary>
        public LanguageOptions LanguageOptions { get; set; }

        /// <summary>
        /// Gets or sets tables that should be excluded from; can include schema.
        /// </summary>
        public List<string> ExcludedTables { get; set; }

        /// <summary>
        /// Gets or sets Template data to pass in to template creation.
        /// </summary>
        public IDictionary<string, object> TemplateData { get; set; }

        /// <summary>
        /// Gets or sets an assembly to read embedded templates from (optional).
        /// </summary>
        public Assembly EmbeddedTemplatesAssembly { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the embedded templates to read (optional).
        /// </summary>
        public string EmbeddedTemplatesNamespace { get; set; }

        /// <summary>
        /// Gets or sets if schema folders are created for table entity classes as per db schema naming.
        /// </summary>
        public bool EnableSchemaFolders { get; set; }

        /// <summary>
        /// Gets or sets whether entity properties are declared and instantiated in the C# 8.0+ nullable reference types style (optional).
        /// https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
        /// https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types
        /// </summary>
        [Obsolete("Deprecated in favor of support for nullable reference types in EF Core 6.", true)]
        public bool EnableNullableReferenceTypes { get; set; }

        /// <summary>
        /// Gets or sets whether table and column descriptions generate XML comments.
        /// </summary>
        public bool GenerateComments { get; set; } = true;
    }
}
