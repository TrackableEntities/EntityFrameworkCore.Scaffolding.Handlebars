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
    /// <summary>
    /// A category of products
    /// </summary>
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }

        /// <summary>
        /// The name of a category
        /// </summary>
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
";

            public const string ProductClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; }
        public byte[] RowVersion { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
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
using Microsoft.EntityFrameworkCore;

namespace FakeNamespace
{
    /// <summary>
    /// A category of products
    /// </summary>
    [Table(""Category"")]
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// The name of a category
        /// </summary>
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; }

        [InverseProperty(nameof(Product.Category))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
";

            public const string ProductClass =
                @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FakeNamespace
{
    [Table(""Product"")]
    [Index(nameof(CategoryId), Name = ""IX_Product_CategoryId"")]
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }
        [Column(TypeName = ""money"")]
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; }
        public byte[] RowVersion { get; set; }
        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty(""Products"")]
        public virtual Category Category { get; set; }
    }
}
";
        }

        private static class ExpectedEntitiesNoEncoding
        {
            public const string CategoryClass =
                @"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    /// <summary>
    /// 产品
    /// </summary>
    public partial class Category : Entity<int>
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }

        /// <summary>
        /// The name of a category
        /// </summary>
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
";
        }

        private static class ExpectedEntitiesWithAnnotationsNoEncoding
        {
            public const string CategoryClass =
                @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FakeNamespace
{
    /// <summary>
    /// 产品
    /// </summary>
    [Table(""Category"")]
    public partial class Category : Entity<int>
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// The name of a category
        /// </summary>
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; }

        [InverseProperty(nameof(Product.Category))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
";
        }

        private static class ExpectedEntitiesWithNullableNavigation
        {
            public const string CategoryClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    /// <summary>
    /// A category of products
    /// </summary>
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }

        /// <summary>
        /// The name of a category
        /// </summary>
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
";

            public const string ProductClass =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; }
        public byte[]? RowVersion { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
    }
}
";
        }
    }
}