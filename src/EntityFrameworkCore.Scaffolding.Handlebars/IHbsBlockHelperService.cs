using System;
using System.Collections.Generic;
using HandlebarsDotNet;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to register Handlebars block helpers.
    /// </summary>
    public interface IHbsBlockHelperService
    {
        /// <summary>
        /// Handlebars block helpers.
        /// </summary>
        Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>> Helpers { get; }

        /// <summary>
        /// Register Handlebars block helpers.
        /// </summary>
        void RegisterBlockHelpers();
    }
}