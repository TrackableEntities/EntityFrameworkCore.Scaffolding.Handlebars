using System.Collections.Generic;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services to obtain template file information.
    /// </summary>
    public interface ITemplateLanguageService
    {
        /// <summary>
        /// Get DbContext template file information.
        /// </summary>
        /// <returns>Dictionary of templates with file information.</returns>
        Dictionary<string, TemplateFileInfo> GetDbContextTemplateFileInfo();

        /// <summary>
        /// Get Entities template file information.
        /// </summary>
        /// <returns>Dictionary of templates with file information.</returns>
        Dictionary<string, TemplateFileInfo> GetEntitiesTemplateFileInfo();
    }
}