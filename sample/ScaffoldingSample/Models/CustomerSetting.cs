using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class CustomerSetting // My Handlebars Helper
    {
        public string CustomerId { get; set; }
        public string Setting { get; set; }

        public virtual Customer Customer { get; set; }

        // The following should output True
        //True

        // The following should output False
        //False
    }
}
