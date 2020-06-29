using System;
using Microsoft.EntityFrameworkCore;
using ScaffoldingSample.Models;
using dbo = ScaffoldingSample.Models.dbo;

namespace ScaffoldingSample.Contexts
{
    public partial class NorthwindSlimContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dbo.Employee>()
                .Property(e => e.Country)
                .HasConversion(
                    v => v.ToString(),
                    v => (Country)Enum.Parse(typeof(Country), v));

            modelBuilder.Entity<dbo.Customer>()
                .Property(e => e.Country)
                .HasConversion(
                    v => v.ToString(),
                    v => (Country)Enum.Parse(typeof(Country), v));
        }
    }
}