using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;

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
        Dictionary<string, Action<TextWriter, HelperOptions, Dictionary<string, object>, object[]>> Helpers { get; }

        /// <summary>
        /// Register Handlebars block helpers.
        /// </summary>
        void RegisterBlockHelpers();
    }
}