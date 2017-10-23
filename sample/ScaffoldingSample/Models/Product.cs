using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Product
    {
        public Product()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? CategoryId { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool? Discontinued { get; set; }
        public byte[] RowVersion { get; set; }

        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
