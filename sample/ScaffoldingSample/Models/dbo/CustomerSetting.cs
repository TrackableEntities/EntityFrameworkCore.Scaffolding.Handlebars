using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class CustomerSetting : EntityBase // My Handlebars Helper
    {
        public string CustomerId { get; set; } = null!;
        public string Setting { get; set; } = null!;

        public virtual Customer Customer { get; set; } = null!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
