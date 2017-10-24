using System.IO;
using EntityFrameworkCore.Scaffolding.Handlebars;
using Scaffolding.Handlebars.Tests.Helpers;
using Xunit;

namespace Scaffolding.Handlebars.Tests
{
    public partial class FileSystemTemplateFileServiceTests
    {
        private readonly string _projectRootDir = Path.Combine("..", "..", "..");

        public FileSystemTemplateFileServiceTests()
        {
            var templatesAltFolder = Path.Combine(_projectRootDir, Constants.Templates.CodeTemplatesAltFolder);
            if (Directory.Exists(templatesAltFolder))
            {
                Directory.Delete(templatesAltFolder, true);
            }
        }

        [Theory]
        [InlineData(
            Constants.Templates.ContextFolder,
            Constants.Templates.ContextClassFile,
            ExpectedTemplates.ContextTemplate)]
        [InlineData(
            Constants.Templates.EntityTypeFolder,
            Constants.Templates.EntityClassFile,
            ExpectedTemplates.EntityTemplate)]
        public void Template_File_Service_Should_Retrieve_Template(
            string templateFolderName, string templateFileName, string expectedTemplate)
        {
            // Act
            var templateExisting = RetrieveTemplate(templateFolderName, templateFileName, true);
            var templateNonExisting = RetrieveTemplate(templateFolderName, templateFileName, false);

            // Assert
            Assert.Equal(expectedTemplate, templateExisting);
            Assert.Equal(expectedTemplate, templateNonExisting);
        }

        private string RetrieveTemplate(string templateFolderName, string templateFileName, bool useExisting)
        {
            // Arrange
            string localDirectory = Path.Combine(_projectRootDir,
                Constants.Templates.CodeTemplatesFolder, templateFolderName);
            string altDirectory = useExisting
                ? localDirectory
                : Path.Combine(_projectRootDir,
                    Constants.Templates.CodeTemplatesAltFolder, templateFolderName);
            string relativeDirectory = $"{Constants.Templates.CodeTemplatesFolder}" +
                                       $"/{templateFolderName}";

            var templateService = new FileSystemTemplateFileService();

            // Act
            return templateService.RetrieveTemplateFileContents(
                relativeDirectory, templateFileName, altDirectory);
        }
    }
}