using Microsoft.EntityFrameworkCore;
using CheckAnalysis.Models;

namespace CheckAnalysis.Data
{
    public class CheckDbContext : DbContext
    {
        public CheckDbContext(DbContextOptions<CheckDbContext> options)
    : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<CheckData> CheckData { get; set; }
    }
}
