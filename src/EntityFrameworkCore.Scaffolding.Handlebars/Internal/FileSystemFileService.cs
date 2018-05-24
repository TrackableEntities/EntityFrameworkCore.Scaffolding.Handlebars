// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

using System;
using System.IO;
using System.Text;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class FileSystemFileService : IFileService
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool DirectoryExists(string directoryName)
        {
            return Directory.Exists(directoryName);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool FileExists(string directoryName, string fileName)
        {
            return File.Exists(Path.Combine(directoryName, fileName));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool IsFileReadOnly(string directoryName, string fileName)
        {
            string path = Path.Combine(directoryName, fileName);
            if (File.Exists(path))
                return File.GetAttributes(path).HasFlag((Enum)FileAttributes.ReadOnly);
            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string RetrieveFileContents(string directoryName, string fileName)
        {
            return File.ReadAllText(Path.Combine(directoryName, fileName));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string OutputFile(string directoryName, string fileName, string contents)
        {
            Directory.CreateDirectory(directoryName);
            string path = Path.Combine(directoryName, fileName);
            File.WriteAllText(path, contents, Encoding.UTF8);
            return path;
        }
    }
}