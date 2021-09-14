using System;
using System.Collections.Generic;
using HandlebarsDotNet;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to register Handlebars helpers.
    /// </summary>
    public interface IHbsHelperService
    {
        /// <summary>
        /// Handlebars helpers.
        /// </summary>
        Dictionary<string, Action<EncodedTextWriter, Context, Arguments>> Helpers { get; }

        /// <summary>
        /// Register Handlebars helpers.
        /// </summary>
        void RegisterHelpers();
    }
}