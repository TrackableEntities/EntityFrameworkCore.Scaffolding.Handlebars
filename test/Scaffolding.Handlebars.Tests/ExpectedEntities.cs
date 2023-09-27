using Scaffolding.Handlebars.Tests.Helpers;

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

        private static class ExpectedEntitiesWithTransformMappings
        {
            public const string CategoryClassConst =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{{
    /// <summary>
    /// A category of products
    /// </summary>
    public partial class {0}
    {{
        public {0}()
        {{
            Products = new HashSet<{1}>();
        }}

        public int CategoryIdRenamed {{ get; set; }}

        /// <summary>
        /// The name of a category
        /// </summary>
        public string CategoryNameRenamed {{ get; set; }}

        public virtual ICollection<{1}> Products {{ get; set; }}
    }}
}}
";

            public const string ProductClassConst =
@"using System;
using System.Collections.Generic;

namespace FakeNamespace
{{
    public partial class {0}
    {{
        public int ProductIdRenamed {{ get; set; }}
        public string ProductName {{ get; set; }}
        public decimal? UnitPriceRenamed {{ get; set; }}
        public bool Discontinued {{ get; set; }}
        public byte[] RowVersion {{ get; set; }}
        public int? CategoryIdRenamed {{ get; set; }}

        public virtual {1} Category {{ get; set; }}
    }}
}}
";

            public static readonly string CategoryClass = string.Format(CategoryClassConst, Constants.Names.Transformed.Category, Constants.Names.Transformed.Product);
            public static readonly string CategoryClassTransformed2 = string.Format(CategoryClassConst, Constants.Names.Transformed2.Category, Constants.Names.Transformed2.Product);

            public static readonly string ProductClass = string.Format(ProductClassConst, Constants.Names.Transformed.Product, Constants.Names.Transformed.Category);
            public static readonly string ProductClassTransformed2 = string.Format(CategoryClassConst, Constants.Names.Transformed2.Product, Constants.Names.Transformed2.Product);
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

            public const string ProductClass =
                @"using System;
using System.Collections.Generic;

namespace FakeNamespace
{
    /// <summary>
    /// 产品
    /// </summary>
    public partial class Product : Entity<int>
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

        private static class ExpectedEntitiesWithAnnotationsAndTransformMappings
        {
            public const string CategoryClassConst =
                @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FakeNamespace
{{
    /// <summary>
    /// A category of products
    /// </summary>
    [Table(""Category"")]
    public partial class {0}
    {{
        public {0}()
        {{
            Products = new HashSet<{1}>();
        }}

        [Key]
        [Column(""CategoryId"")]
        public int CategoryIdRenamed {{ get; set; }}

        /// <summary>
        /// The name of a category
        /// </summary>
        [Required]
        [Column(""CategoryName"")]
        [StringLength(15)]
        public string CategoryNameRenamed {{ get; set; }}

        [InverseProperty(nameof({1}.Category))]
        public virtual ICollection<{1}> Products {{ get; set; }}
    }}
}}
";

            public const string ProductClassConst =
                @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FakeNamespace
{{
    [Table(""Product"")]
    [Index(nameof(CategoryId), Name = ""IX_Product_CategoryId"")]
    public partial class {0}
    {{
        [Key]
        [Column(""ProductId"")]
        public int ProductIdRenamed {{ get; set; }}
        [Required]
        [StringLength(40)]
        public string ProductName {{ get; set; }}
        [Column(""UnitPrice"", TypeName = ""money"")]
        public decimal? UnitPriceRenamed {{ get; set; }}
        public bool Discontinued {{ get; set; }}
        public byte[] RowVersion {{ get; set; }}
        [Column(""CategoryId"")]
        public int? CategoryIdRenamed {{ get; set; }}

        [ForeignKey(nameof(CategoryIdRenamed))]
        [InverseProperty(""Products"")]
        public virtual {1} Category {{ get; set; }}
    }}
}}
";

            public static readonly string CategoryClass = string.Format(CategoryClassConst, Constants.Names.Transformed.Category, Constants.Names.Transformed.Product);
            public static readonly string CategoryClassTransformed2 = string.Format(CategoryClassConst, Constants.Names.Transformed2.Category, Constants.Names.Transformed2.Product);

            public static readonly string ProductClass = string.Format(ProductClassConst, Constants.Names.Transformed.Product, Constants.Names.Transformed.Category);
            public static readonly string ProductClassTransformed2 = string.Format(CategoryClassConst, Constants.Names.Transformed2.Product, Constants.Names.Transformed2.Product);

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

            public const string ProductClass =
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
    [Table(""Product"")]
    [Index(nameof(CategoryId), Name = ""IX_Product_CategoryId"")]
    public partial class Product : Entity<int>
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