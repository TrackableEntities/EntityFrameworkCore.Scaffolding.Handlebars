using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class Category : EntityBase // My Handlebars Helper
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; } = default!;
        public string CategoryName { get; set; } = default!;

        public virtual ICollection<Product> Products { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
