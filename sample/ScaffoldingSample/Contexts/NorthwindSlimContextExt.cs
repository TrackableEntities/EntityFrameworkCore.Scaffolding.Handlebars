using System;
using Microsoft.EntityFrameworkCore;

namespace ScaffoldingSample.Models
{
    public partial class NorthwindSlimContext
    {
        partial void OnModelCreatingExt(ModelBuilder modelBuilder)
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