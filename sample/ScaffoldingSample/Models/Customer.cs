using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Customer // My Handlebars Helper
    {
        public Customer()
        {
            Order = new HashSet<Order>();
        }

        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }

        public virtual CustomerSetting CustomerSetting { get; set; }
        public virtual ICollection<Order> Order { get; set; }

        // The following should output True
        //True

        // The following should output False
        //False
    }
}
