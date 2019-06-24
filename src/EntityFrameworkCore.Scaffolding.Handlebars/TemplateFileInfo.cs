namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Template file information.
    /// </summary>
    public class TemplateFileInfo
    {
        /// <summary>
        /// Relative directory name.
        /// </summary>
        public string RelativeDirectory { get; set; }

        /// <summary>
        /// File name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Alternative relative directory. Used for testing purposes.
        /// </summary>
        public string AltRelativeDirectory { get; set; }
    }
}
