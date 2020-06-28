using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;

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

        /// <summary>
        /// Find all files in relative directory
        /// </summary>
        /// <param name="relativeDirectory">Relative Directory.</param>
        /// <returns></returns>
        public virtual string[] RetrieveAllFileNames(string relativeDirectory)
        {            
            return Directory.GetFiles(relativeDirectory, $"*{Constants.TemplateExtension}").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
        }

        /// <summary>
        /// Finds all partial templates
        /// </summary>
        /// <param name="result">Dictionary containing template info</param>
        /// <param name="relativeDirectory">Relative Directory.</param>
        /// <returns></returns>
        public virtual Dictionary<string, TemplateFileInfo> FindAllPartialTemplates(Dictionary<string, TemplateFileInfo> result, string relativeDirectory)
        {
            foreach (var file in RetrieveAllFileNames(relativeDirectory))
            {
                result.Add(file, new TemplateFileInfo()
                {
                    RelativeDirectory = relativeDirectory,
                    FileName = file + Constants.TemplateExtension
                });
            }
            return result;
        }

        /// <summary>
        /// Retreive template file contents from the file system. 
        /// If template is not present, copy it locally.
        /// </summary>
        /// <param name="relativeDirectory">Relative directory name.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="altRelativeDirectory">Alternative relative directory. Used for testing purposes.</param>
        /// <returns>File contents.</returns>
        public virtual string RetrieveTemplateFileContents(string relativeDirectory, string fileName,
            string altRelativeDirectory = null)
        {
            string contents;
            string directory = altRelativeDirectory ?? relativeDirectory;
            string path = Path.Combine(directory, fileName);
            if (File.Exists(path))
            {
                contents = RetrieveFileContents(directory, fileName);
            }
            else
            {
                var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var localDirectory = Path.Combine(assemblyDirectory, relativeDirectory);
                var templateContents = RetrieveFileContents(localDirectory, fileName);
                OutputFile(directory, fileName, templateContents);
                contents = RetrieveFileContents(directory, fileName);
            }
            return contents;
        }
    }
}
