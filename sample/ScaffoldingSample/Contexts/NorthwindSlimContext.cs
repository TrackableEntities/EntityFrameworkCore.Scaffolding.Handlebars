using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class NorthwindSlimContext : DbContext
    {
        // My Handlebars Helper
        public virtual DbSet<Category> Category { get; set; }
        // My Handlebars Helper
        public virtual DbSet<Customer> Customer { get; set; }
        // My Handlebars Helper
        public virtual DbSet<CustomerSetting> CustomerSetting { get; set; }
        // My Handlebars Helper
        public virtual DbSet<Employee> Employee { get; set; }
        // My Handlebars Helper
        public virtual DbSet<EmployeeTerritories> EmployeeTerritories { get; set; }
        // My Handlebars Helper
        public virtual DbSet<Order> Order { get; set; }
        // My Handlebars Helper
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        // My Handlebars Helper
        public virtual DbSet<Product> Product { get; set; }
        // My Handlebars Helper
        public virtual DbSet<Territory> Territory { get; set; }

        public NorthwindSlimContext(DbContextOptions<NorthwindSlimContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(15);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .ValueGeneratedNever();

                entity.Property(e => e.City).HasMaxLength(15);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.ContactName).HasMaxLength(30);

                entity.Property(e => e.Country).HasMaxLength(15);
            });

            modelBuilder.Entity<CustomerSetting>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK_dbo.CustomerSetting");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .ValueGeneratedNever();

                entity.Property(e => e.Setting)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.CustomerSetting)
                    .HasForeignKey<CustomerSetting>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerSetting_Customer");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(15);

                entity.Property(e => e.Country).HasMaxLength(15);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.HireDate).HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<EmployeeTerritories>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.TerritoryId })
                    .HasName("PK_dbo.EmployeeTerritories");

                entity.Property(e => e.TerritoryId).HasMaxLength(20);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeTerritories)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_dbo.EmployeeTerritories_dbo.Employee_EmployeeId");

                entity.HasOne(d => d.Territory)
                    .WithMany(p => p.EmployeeTerritories)
                    .HasForeignKey(d => d.TerritoryId)
                    .HasConstraintName("FK_dbo.EmployeeTerritories_dbo.Territory_TerritoryId");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CustomerId).HasMaxLength(5);

                entity.Property(e => e.Freight)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.ShippedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customers");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Quantity).HasDefaultValueSql("((1))");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_Categories");
            });

            modelBuilder.Entity<Territory>(entity =>
            {
                entity.Property(e => e.TerritoryId)
                    .HasMaxLength(20)
                    .ValueGeneratedNever();

                entity.Property(e => e.TerritoryDescription)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingExt(modelBuilder);
        }

        partial void OnModelCreatingExt(ModelBuilder modelBuilder);
    }
}
