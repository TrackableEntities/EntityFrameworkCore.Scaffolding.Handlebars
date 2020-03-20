using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using System.Collections.Generic;

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

        /// <summary>
        /// Retreive template file contents from the file system. 
        /// If template is not present, copy it locally.
        /// </summary>
        /// <param name="relativeDirectory">Relative directory name.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="altRelativeDirectory">Alternative relative directory. Used for testing purposes.</param>
        /// <returns>File contents.</returns>
        string RetrieveTemplateFileContents(string relativeDirectory, string fileName,
            string altRelativeDirectory = null);

        /// <summary>
        /// Retries all files from a relative directory.
        /// </summary>
        /// <param name="relativeDirectory">Relative directory name.</param>
        /// <returns>File names.</returns>
        string[] RetrieveAllFileNames(string relativeDirectory);

        /// <summary>
        /// Finds all partial templates
        /// </summary>
        /// <param name="result">Dictionary containing template info</param>
        /// <param name="relativeDirectory">Relative Directory.</param>
        /// <returns></returns>
        Dictionary<string, TemplateFileInfo> FindAllPartialTemplates(Dictionary<string, TemplateFileInfo> result, string relativeDirectory);
    }
}