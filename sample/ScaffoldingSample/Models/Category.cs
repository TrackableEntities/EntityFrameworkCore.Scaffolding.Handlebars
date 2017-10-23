using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
