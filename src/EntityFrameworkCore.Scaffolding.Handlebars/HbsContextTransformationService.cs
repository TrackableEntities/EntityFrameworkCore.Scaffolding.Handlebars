using System;
namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Default service for transforming context definitions.
    /// </summary>
    public class HbsContextTransformationService : IContextTransformationService
    {
        /// <summary>
        /// Context file name transformer.
        /// </summary>
        public Func<string, string> ContextFileNameTransformer { get; }

        /// <summary>
        /// HbsContextTransformationService constructor.
        /// </summary>
        /// <param name="contextFileNameTransformer">Context file name transformer.</param>
        public HbsContextTransformationService(
            Func<string, string> contextFileNameTransformer = null)
        {
            ContextFileNameTransformer = contextFileNameTransformer;
        }

        /// <summary>
        /// Transform context file name.
        /// </summary>
        /// <param name="contextFileName">Context file name.</param>
        /// <returns>Transformed context file name.</returns>
        public string TransformContextFileName(string contextFileName) =>
            ContextFileNameTransformer?.Invoke(contextFileName) ?? contextFileName;
    }
}