// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provides files to the template service from the file system.
    /// </summary>
    public class FileSystemTemplateFileService : FileSystemFileService, ITemplateFileService
    {
        /// <summary>
        /// Allows files to be stored for later retrieval. Used for testing purposes.
        /// </summary>
        /// <param name="files">Files used by the template service.</param>
        /// <returns>Array of file paths.</returns>
        public virtual string[] InputFiles(params InputFile[] files)
        {
            var filePaths = new List<string>();

            foreach (var file in files)
            {
                var path = Path.Combine(file.Directory, file.File);
                filePaths.Add(path);
            }

            return filePaths.ToArray();
        }
    }
}
