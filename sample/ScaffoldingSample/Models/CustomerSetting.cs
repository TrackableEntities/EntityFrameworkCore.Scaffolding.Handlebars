using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class CustomerSetting
    {
        public string CustomerId { get; set; }
        public string Setting { get; set; }

        public Customer Customer { get; set; }
    }
}
