using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Territory // My Handlebars Helper
    {
        public Territory()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
        }

        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; set; }

        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
