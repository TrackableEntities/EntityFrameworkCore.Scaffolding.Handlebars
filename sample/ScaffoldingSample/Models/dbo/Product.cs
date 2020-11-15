using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class Product : EntityBase // My Handlebars Helper
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int? CategoryId { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; } = default!;
        public byte[]? RowVersion { get; set; }

        public virtual Category Category { get; set; } = default!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
