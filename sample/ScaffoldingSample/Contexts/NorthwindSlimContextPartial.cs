using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
using dbo = ScaffoldingSample.Models.dbo;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace ScaffoldingSample.Contexts
{
    public partial class NorthwindSlimContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dbo.Employee>()
                .Property(e => e.Country)
                .HasConversion<string>();
            modelBuilder.Entity<dbo.Customer>()
                .Property(e => e.Country)
                .HasConversion<string>();
        }
    }
}