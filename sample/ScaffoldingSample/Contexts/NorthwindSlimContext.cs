using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata; // Comment
using dbo = ScaffoldingSample.Models.dbo;

namespace ScaffoldingSample.Contexts
{ // Comment
    public partial class NorthwindSlimContext : DbContext
    {
        // My Handlebars Helper
        public virtual DbSet<dbo.Category> Categories { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.Customer> Customers { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.CustomerSetting> CustomerSettings { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.Employee> Employees { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.Order> Orders { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.OrderDetail> OrderDetails { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.Product> Products { get; set; } = null!;
        // My Handlebars Helper
        public virtual DbSet<dbo.Territory> Territories { get; set; } = null!;

        public NorthwindSlimContext()
        {
        }

        public NorthwindSlimContext(DbContextOptions<NorthwindSlimContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=NorthwindSlim; Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dbo.Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryName).HasMaxLength(15);
            });

            modelBuilder.Entity<dbo.Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).HasMaxLength(5);

                entity.Property(e => e.City).HasMaxLength(15);

                entity.Property(e => e.CompanyName).HasMaxLength(40);

                entity.Property(e => e.ContactName).HasMaxLength(30);

                entity.Property(e => e.Country).HasMaxLength(15);
            });

            modelBuilder.Entity<dbo.CustomerSetting>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK_dbo.CustomerSetting");

                entity.ToTable("CustomerSetting");

                entity.Property(e => e.CustomerId).HasMaxLength(5);

                entity.Property(e => e.Setting).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.CustomerSetting)
                    .HasForeignKey<dbo.CustomerSetting>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerSetting_Customer");
            });

            modelBuilder.Entity<dbo.Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(15);

                entity.Property(e => e.Country).HasMaxLength(15);

                entity.Property(e => e.FirstName).HasMaxLength(20);

                entity.Property(e => e.HireDate).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(20);

                entity.HasMany(d => d.Territories)
                    .WithMany(p => p.Employees)
                    .UsingEntity<Dictionary<string, object>>(
                        "EmployeeTerritory",
                        l => l.HasOne<dbo.Territory>().WithMany().HasForeignKey("TerritoryId").HasConstraintName("FK_dbo.EmployeeTerritories_dbo.Territory_TerritoryId"),
                        r => r.HasOne<dbo.Employee>().WithMany().HasForeignKey("EmployeeId").HasConstraintName("FK_dbo.EmployeeTerritories_dbo.Employee_EmployeeId"),
                        j =>
                        {
                            j.HasKey("EmployeeId", "TerritoryId").HasName("PK_dbo.EmployeeTerritories");

                            j.ToTable("EmployeeTerritories");

                            j.IndexerProperty<string>("TerritoryId").HasMaxLength(20);
                        });
            });

            modelBuilder.Entity<dbo.Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.CustomerId).HasMaxLength(5);

                entity.Property(e => e.Freight)
                    .HasColumnType("money")
                    .HasDefaultValue(0m);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.ShippedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customers");
            });

            modelBuilder.Entity<dbo.OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.Discount).HasDefaultValue(0f);

                entity.Property(e => e.Quantity).HasDefaultValue((short)1);

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("money")
                    .HasDefaultValue(0m);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            modelBuilder.Entity<dbo.Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Discontinued).HasDefaultValue(false);

                entity.Property(e => e.ProductName).HasMaxLength(40);

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("money")
                    .HasDefaultValue(0m);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_Categories");
            });

            modelBuilder.Entity<dbo.Territory>(entity =>
            {
                entity.ToTable("Territory");

                entity.Property(e => e.TerritoryId).HasMaxLength(20);

                entity.Property(e => e.TerritoryDescription).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
