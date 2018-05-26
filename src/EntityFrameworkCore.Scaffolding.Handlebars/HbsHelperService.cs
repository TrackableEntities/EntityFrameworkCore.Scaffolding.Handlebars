﻿using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsLib = HandlebarsDotNet.Handlebars;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide services required to register Handlebars helpers.
    /// </summary>
    public class HbsHelperService : IHbsHelperService
    {
        /// <summary>
        /// Handlebars helpers.
        /// </summary>
        public Dictionary<string, Action<TextWriter, object, object[]>> Helpers { get; }

        /// <summary>
        /// Constructor for the Handlebars helper service.
        /// </summary>
        /// <param name="helpers">Dictionary of Handlebars helpers.</param>
        public HbsHelperService(
            Dictionary<string, Action<TextWriter, object, object[]>> helpers)
        {
            Helpers = helpers;
        }

        /// <summary>
        /// Register Handlebars helpers.
        /// </summary>
        public void RegisterHelpers()
        {
            foreach (var helper in Helpers)
                HandlebarsLib.RegisterHelper(helper.Key,
                    (output, context, args) => helper.Value(output, context, args));
        }
    }
}