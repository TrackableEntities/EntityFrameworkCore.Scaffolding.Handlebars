// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class HbsCSharpScaffoldingGenerator : ScaffoldingCodeGenerator
    {
        public virtual IEntityTypeTemplateService EntityTypeTemplateService { get; }
        public virtual ICSharpDbContextGenerator CSharpDbContextGenerator { get; }
        public virtual ICSharpEntityTypeGenerator CSharpEntityTypeGenerator { get; }

        public HbsCSharpScaffoldingGenerator(
            ITemplateFileService fileService,
            IEntityTypeTemplateService entityTypeTemplateService,
            ICSharpDbContextGenerator cSharpDbContextGenerator,
            ICSharpEntityTypeGenerator cSharpEntityTypeGenerator)
          : base(fileService)
        {
            EntityTypeTemplateService = entityTypeTemplateService ?? throw new ArgumentNullException(nameof(entityTypeTemplateService));
            CSharpDbContextGenerator = cSharpDbContextGenerator ?? throw new ArgumentNullException(nameof(cSharpDbContextGenerator));
            CSharpEntityTypeGenerator = cSharpEntityTypeGenerator ?? throw new ArgumentNullException(nameof(cSharpEntityTypeGenerator));
        }

        public override string FileExtension => ".cs";

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

            // Register Hbs helpers and partial templates
            EntityTypeTemplateService.RegisterHelper(Constants.SpacesHelper, HandlebarsHelpers.GetSpacesHelper());
            EntityTypeTemplateService.RegisterPartialTemplates();

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
