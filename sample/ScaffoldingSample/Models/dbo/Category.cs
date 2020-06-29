using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class Category : EntityBase // My Handlebars Helper
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; } = default!;
        public string CategoryName { get; set; } = default!;

        public virtual ICollection<Product> Product { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
