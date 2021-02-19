using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class Customer : EntityBase // My Handlebars Helper
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public string? ContactName { get; set; }
        public string? City { get; set; }
        public Country Country { get; set; } = default!;

        public virtual CustomerSetting? CustomerSetting { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
