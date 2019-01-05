using System;
using System.Collections.Generic;
using System.IO;

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
        Dictionary<string, Action<TextWriter, Dictionary<string, object>, object[]>> Helpers { get; }

        /// <summary>
        /// Register Handlebars helpers.
        /// </summary>
        void RegisterHelpers();
    }
}