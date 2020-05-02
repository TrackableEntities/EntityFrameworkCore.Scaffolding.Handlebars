using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata; // Comment
using ScaffoldingSample.Models;

namespace ScaffoldingSample.Contexts
{ // Comment
    public partial class NorthwindSlimContext : DbContext
    {
        // My Handlebars Helper
        public virtual DbSet<Category> Category { get; set; } = default!;
        // My Handlebars Helper
        public virtual DbSet<Customer> Customer { get; set; } = default!;
        // My Handlebars Helper
        public virtual DbSet<CustomerSetting> CustomerSetting { get; set; } = default!;
        // My Handlebars Helper
        public virtual DbSet<Employee> Employee { get; set; } = default!;
        // My Handlebars Helper
        public virtual DbSet<Order> Order { get; set; } = default!;
        // My Handlebars Helper
        public virtual DbSet<OrderDetail> OrderDetail { get; set; } = default!;
        // My Handlebars Helper
        public virtual DbSet<Product> Product { get; set; } = default!;

        public NorthwindSlimContext(DbContextOptions<NorthwindSlimContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                    .IsFixedLength();

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
                    .IsFixedLength();

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

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .IsFixedLength();

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
