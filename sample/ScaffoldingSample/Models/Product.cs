using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Product : EntityBase // My Handlebars Helper
    {
        public Product()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int? CategoryId { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; } = default!;
        public byte[]? RowVersion { get; set; }

        public virtual Category Category { get; set; } = default!;
        public virtual ICollection<OrderDetail> OrderDetail { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
