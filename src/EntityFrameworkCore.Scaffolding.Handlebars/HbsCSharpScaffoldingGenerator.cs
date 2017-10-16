// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HbsCSharpScaffoldingGenerator : ScaffoldingCodeGenerator
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ICSharpDbContextGenerator CSharpDbContextGenerator { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ICSharpEntityTypeGenerator CSharpEntityTypeGenerator { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HbsCSharpScaffoldingGenerator(
            IFileService fileService, 
            ICSharpDbContextGenerator cSharpDbContextGenerator, 
            ICSharpEntityTypeGenerator cSharpEntityTypeGenerator)
          : base(fileService)
        {
            CSharpDbContextGenerator = cSharpDbContextGenerator ?? throw new ArgumentNullException(nameof(cSharpDbContextGenerator));
            CSharpEntityTypeGenerator = cSharpEntityTypeGenerator ?? throw new ArgumentNullException(nameof(cSharpEntityTypeGenerator));
        }

        public override string FileExtension => ".cs";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ReverseEngineerFiles WriteCode(
            IModel model, 
            string outputPath, 
            string @namespace, 
            string contextName, 
            string connectionString, 
            bool useDataAnnotations)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (outputPath == null) throw new ArgumentNullException(nameof(outputPath));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));
            if (contextName == null) throw new ArgumentNullException(nameof(contextName));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            ReverseEngineerFiles reverseEngineerFiles = new ReverseEngineerFiles();
            string contents1 = CSharpDbContextGenerator.WriteCode(model, @namespace, contextName, connectionString, useDataAnnotations);
            string fileName1 = contextName + FileExtension;
            string str1 = FileService.OutputFile(outputPath, fileName1, contents1);
            reverseEngineerFiles.ContextFile = str1;
            foreach (IEntityType entityType in model.GetEntityTypes())
            {
                string contents2 = CSharpEntityTypeGenerator.WriteCode(entityType, @namespace, useDataAnnotations);
                string fileName2 = entityType.DisplayName() + FileExtension;
                string str2 = FileService.OutputFile(outputPath, fileName2, contents2);
                reverseEngineerFiles.EntityTypeFiles.Add(str2);
            }
            return reverseEngineerFiles;
        }
    }
}
