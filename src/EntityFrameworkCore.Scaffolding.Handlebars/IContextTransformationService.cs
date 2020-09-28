namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Service for transforming context definitions.
    /// </summary>
    public interface IContextTransformationService
    {
        /// <summary>
        /// Transform context file name.
        /// </summary>
        /// <param name="contextFileName">Context file name.</param>
        /// <returns>Transformed entity file name.</returns>
        string TransformContextFileName(string contextFileName);
    }
}