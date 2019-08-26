using System;
using Microsoft.EntityFrameworkCore;
using ScaffoldingSample.Models;

namespace ScaffoldingSample.Contexts
{
    public partial class NorthwindSlimContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.Country)
                .HasConversion(
                    v => v.ToString(),
                    v => (Country)Enum.Parse(typeof(Country), v));

            modelBuilder.Entity<Customer>()
                .Property(e => e.Country)
                .HasConversion(
                    v => v.ToString(),
                    v => (Country)Enum.Parse(typeof(Country), v));
        }
    }
}