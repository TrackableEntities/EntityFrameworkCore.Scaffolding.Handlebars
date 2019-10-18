using System;
using System.Collections.Generic;

namespace ScaffoldingSample.Embedded.Models
{
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
        public bool Discontinued { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
