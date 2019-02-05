using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Design;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Options for generating scaffolding with handlebars.
    /// </summary>
    public class HandlebarsScaffoldingOptions
    {
        /// <summary>
        /// Gets or sets tables that should be excluded from; can include schema. 
        /// </summary>
        public List<string> ExcludedTables { get; set; }

        /// <summary>
        /// Gets or sets which type of scaffolding should be generated.
        /// </summary>
        public ScaffoldingGeneration ScaffoldingGeneration { get; set; }
    }
}
