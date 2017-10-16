// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    public interface ITemplateFileService : IFileService
    {
        string[] InputFiles((string directoryName, string fileName, string contents)[] files);
    }
}