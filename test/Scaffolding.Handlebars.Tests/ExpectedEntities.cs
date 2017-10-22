namespace Scaffolding.Handlebars.Tests
{
    public partial class HbsCSharpScaffoldingGeneratorTests
    {
        private static class ExpectedEntities
        {
            public const string CategoryClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
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
";

            public const string ProductClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
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
";
        }

        private static class ExpectedEntitiesWithAnnotations
        {
            public const string CategoryClass =
                @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeNamespace
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; }

        [InverseProperty(""Category"")]
        public ICollection<Product> Product { get; set; }
    }
}
";

            public const string ProductClass =
                @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeNamespace
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public bool Discontinued { get; set; }
        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }
        public byte[] RowVersion { get; set; }
        [Column(TypeName = ""money"")]
        public decimal? UnitPrice { get; set; }

        [ForeignKey(""CategoryId"")]
        [InverseProperty(""Product"")]
        public Category Category { get; set; }
    }
}
";
        }
    }
}