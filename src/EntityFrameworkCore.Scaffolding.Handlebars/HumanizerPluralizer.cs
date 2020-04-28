﻿using Humanizer;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provide methods to implement IPluralizer with Humanizer.
    /// </summary>
    public class HumanizerPluralizer : IPluralizer
    {
        #region Methods

        /// <summary>
        /// Pluralize a string
        /// </summary>
        /// <param name="identifier">String to pluralize</param>
        /// <returns></returns>
        public string Pluralize(string identifier)
        {
            return identifier.Pluralize(false);
        }

        /// <summary>
        /// Singularize a string
        /// </summary>
        /// <param name="identifier">String to singularize</param>
        /// <returns></returns>
        public string Singularize(string identifier)
        {
            return identifier.Singularize(false);
        }

        #endregion
    }
}
