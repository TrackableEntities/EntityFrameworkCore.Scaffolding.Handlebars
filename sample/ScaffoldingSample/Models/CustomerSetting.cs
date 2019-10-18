using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class CustomerSetting : EntityBase // My Handlebars Helper
    {
        public string CustomerId { get; set; }
        public string Setting { get; set; }

        public virtual Customer Customer { get; set; }

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
