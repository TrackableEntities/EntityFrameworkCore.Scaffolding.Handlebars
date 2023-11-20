using System;
using Microsoft.EntityFrameworkCore;
using ScaffoldingSample.Models;
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
using dbo = ScaffoldingSample.Models.dbo;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace ScaffoldingSample.Contexts
{
    public partial class NorthwindSlimContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
// #pragma warning disable CS8604 // Possible null reference argument.
            // modelBuilder.Entity<dbo.Employee>()
            //     .Property(e => e.Country)
            //     .HasConversion(
            //         v => v.ToString(),
            //         v => (Country)Enum.Parse(typeof(Country?), v));
            //
            // modelBuilder.Entity<dbo.Customer>()
            //     .Property(e => e.Country)
            //     .HasConversion(
            //         v => v.ToString(),
            //         v => (Country)Enum.Parse(typeof(Country?), v));
// #pragma warning restore CS8604 // Possible null reference argument.
        }
    }
}