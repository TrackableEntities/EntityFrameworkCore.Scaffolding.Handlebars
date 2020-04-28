using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Pluralizer options for reverse engineering classes from an existing database.
    /// </summary>
    public enum PluralizerOptions
    {
        /// <summary>
        /// Don't use pluralization service
        /// </summary>
        DontUse,

        /// <summary>
        /// Use humanizer implementation of pluralization
        /// </summary>
        UseHumanizerPluralization
    }
}
