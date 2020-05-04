using Constants = Scaffolding.Handlebars.Tests.Helpers.Constants;
namespace Scaffolding.Handlebars.Tests
{
    public partial class HbsCSharpScaffoldingGeneratorTests
    {
        private static class ExpectedContexts
        {
            public static string ContextClass =
@"using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FakeNamespace
{
    public partial class FakeDbContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }

        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(""" + Constants.Connections.SqlServerConnection.Replace(@"\",@"\\") + @""");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasComment(""A category of products"");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasComment(""The name of a category"");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.UnitPrice).HasColumnType(""money"");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
";
        }

        public static class ExpectedContextsWithAnnotations
        {
            public static string ContextClass =
@"using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FakeNamespace
{
    public partial class FakeDbContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }

        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(""" + Constants.Connections.SqlServerConnection.Replace(@"\",@"\\") + @""");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasComment(""A category of products"");

                entity.Property(e => e.CategoryName).HasComment(""The name of a category"");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId);

                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
";
        }

        private static class ExpectedContextsPluralized
        {
            public const string ContextClass =
@"using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FakeNamespace
{
    public partial class FakeDbContext : DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(""Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=NorthwindTestDb; Integrated Security=True"");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable(""Category"");

                entity.HasComment(""A category of products"");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasComment(""The name of a category"");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable(""Product"");

                entity.HasIndex(e => e.CategoryId);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.UnitPrice).HasColumnType(""money"");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
";
        }

        public static class ExpectedContextsWithAnnotationsPluralized
        {
            public const string ContextClass =
@"using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FakeNamespace
{
    public partial class FakeDbContext : DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(""Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=NorthwindTestDb; Integrated Security=True"");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasComment(""A category of products"");

                entity.Property(e => e.CategoryName).HasComment(""The name of a category"");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId);

                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
";
        }

    }
}
