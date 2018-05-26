namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to generate classes using Handlebars templates.
    /// </summary>
    public interface IHbsTemplateService
    {
        /// <summary>
        /// Register partial templates.
        /// </summary>
        void RegisterPartialTemplates();
    }
}