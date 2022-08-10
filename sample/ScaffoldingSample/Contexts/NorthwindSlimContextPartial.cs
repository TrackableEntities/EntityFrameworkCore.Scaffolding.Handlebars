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
        }
    }
}