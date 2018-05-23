// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal
{
    /// <summary>
    /// Abstraction of a file service.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Returns true if directory exists, otherwise returns false.
        /// </summary>
        /// <param name="directoryName">Name of a directory.</param>
        /// <returns>True if directory exists.</returns>
        bool DirectoryExists(string directoryName);

        /// <summary>
        /// Returns true if file exists, otherwise returns false.
        /// </summary>
        /// <param name="directoryName">Name of a directory.</param>
        /// <param name="fileName">Name of a file.</param>
        /// <returns>True if file exists.</returns>
        bool FileExists(string directoryName, string fileName);

        /// <summary>
        /// Returns true if file is read only, otherwise returns false.
        /// </summary>
        /// <param name="directoryName">Name of a directory.</param>
        /// <param name="fileName">Name of a file.</param>
        /// <returns>True if file is read only.</returns>
        bool IsFileReadOnly(string directoryName, string fileName);

        /// <summary>
        /// Name of an output file.
        /// </summary>
        /// <param name="directoryName">Name of a directory.</param>
        /// <param name="fileName">Name of a file.</param>
        /// <param name="contents">File contents.</param>
        /// <returns>Name of an output file.</returns>
        string OutputFile(string directoryName, string fileName, string contents);

        /// <summary>
        /// Retrieve file contents.
        /// </summary>
        /// <param name="directoryName">Name of a directory.</param>
        /// <param name="fileName">Name of a file.</param>
        /// <returns>File contents.</returns>
        string RetrieveFileContents(string directoryName, string fileName);
    }
}