using Microsoft.EntityFrameworkCore;
using Scaffolding.Handlebars.Tests.Models;

namespace Scaffolding.Handlebars.Tests.Contexts
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext() { }

        public NorthwindDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasComment("A category of products")
                .Property(category => category.CategoryName)
                    .HasComment("The name of a category");
            modelBuilder.Entity<Product>()
                .HasComment("产品")
                .Property(e => e.ProductId)
                .HasComment("编号");
            modelBuilder.Entity<Product>()
                .Property(e => e.ProductName)
                .HasComment("名称");
        }
    }
}
