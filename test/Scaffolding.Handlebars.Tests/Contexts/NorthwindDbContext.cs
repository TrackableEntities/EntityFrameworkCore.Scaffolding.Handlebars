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
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasComment("A category of products")
                .Property(category => category.CategoryName)
                    .HasComment("The name of a category");
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerKey).IsFixedLength();
            });
        }
    }
}
