using System;
using System.Collections.Generic;
using HandlebarsDotNet;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to register Handlebars block helpers.
    /// </summary>
    public class HbsBlockHelperService : IHbsBlockHelperService
    {
        /// <summary>
        /// Handlebars block helpers.
        /// </summary>
        public Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>> Helpers { get; }

        /// <summary>
        /// Constructor for the Handlebars block helper service.
        /// </summary>
        /// <param name="helpers">Dictionary of Handlebars helpers.</param>
        public HbsBlockHelperService(Dictionary<string, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>> helpers)
        {
            Helpers = helpers;
        }

        /// <summary>
        /// Register Handlebars block helpers.
        /// </summary>
        public void RegisterBlockHelpers()
        {
            foreach (var helper in Helpers)
                HandlebarsDotNet.Handlebars.RegisterHelper(helper.Key,
                    (output, options, context, args) => helper.Value(output, options, context, args));
        }
    }
}