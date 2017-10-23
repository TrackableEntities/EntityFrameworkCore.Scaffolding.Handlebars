﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provides files to the template service.
    /// </summary>
    public interface ITemplateFileService : IFileService
    {
        /// <summary>
        /// Allows files to be stored for later retrieval. Used for testing purposes.
        /// </summary>
        /// <param name="files">Files used by the template service.</param>
        /// <returns>Array of file paths.</returns>
        string[] InputFiles(params InputFile[] files);
    }
}