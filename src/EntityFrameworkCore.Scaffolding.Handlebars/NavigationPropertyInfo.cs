using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Navigation property info
    /// </summary>
    public class NavigationPropertyInfo: EntityPropertyInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NavigationPropertyInfo() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyIsNullable">Property is nullable.</param>
        /// <param name="navigation">EF Core navigation</param>
        public NavigationPropertyInfo(string propertyType, string propertyName, bool propertyIsNullable = false, INavigation navigation = null)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            PropertyIsNullable = propertyIsNullable;
            NavigationDetails = navigation;
        }


        /// <summary>
        /// EF Core Navigation details
        /// </summary>
        public INavigation NavigationDetails { get; set; }
    }
}
