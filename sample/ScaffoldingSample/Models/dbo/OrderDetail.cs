using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models.dbo
{ // Comment
    public partial class OrderDetail : EntityBase // My Handlebars Helper
    {
        public int OrderDetailId { get; set; } = default!;
        public int OrderId { get; set; } = default!;
        public int ProductId { get; set; } = default!;
        public decimal UnitPrice { get; set; } = default!;
        public short Quantity { get; set; } = default!;
        public float Discount { get; set; } = default!;

        public virtual Order Order { get; set; } = default!;
        public virtual Product Product { get; set; } = default!;

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
