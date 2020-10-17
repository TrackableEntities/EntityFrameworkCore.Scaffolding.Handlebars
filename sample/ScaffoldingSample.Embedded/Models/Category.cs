using System;
using System.Collections.Generic;

namespace ScaffoldingSample.Embedded.Models
{
    /// <summary>
    /// hello table Customer
    /// </summary>
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        /// <summary>
        /// hello CompanyName
        /// </summary>
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
