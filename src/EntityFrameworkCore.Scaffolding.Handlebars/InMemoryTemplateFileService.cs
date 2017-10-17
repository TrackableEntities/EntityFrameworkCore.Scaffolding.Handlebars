// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System.Collections.Generic;
using System.IO;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public class InMemoryTemplateFileService : InMemoryFileService, ITemplateFileService
    {
        public string[] InputFiles(params (string directoryName, string fileName, string contents)[] files)
        {
            var filePaths = new List<string>();

            foreach (var file in files)
            {
                if (!NameToContentMap.TryGetValue(file.directoryName, out var filesMap))
                {
                    filesMap = new Dictionary<string, string>();
                    NameToContentMap[file.directoryName] = filesMap;
                }

                filesMap[file.fileName] = file.contents;

                var path = Path.Combine(file.directoryName, file.fileName);
                filePaths.Add(path);
            }

            return filePaths.ToArray();
        }
    }
}
