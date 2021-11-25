using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class Territory : EntityBase // My Handlebars Helper
    {
        public Territory()
        {
            Employees = new HashSet<Employee>();
        }

        public string TerritoryId { get; set; } = null!;
        public string TerritoryDescription { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
