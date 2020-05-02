using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class CustomerSetting : EntityBase // My Handlebars Helper
    {
        public string CustomerId { get; set; } = default!;
        public string Setting { get; set; } = default!;

        public virtual Customer Customer { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
