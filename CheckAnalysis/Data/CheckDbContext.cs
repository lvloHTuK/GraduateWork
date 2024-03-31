using Microsoft.EntityFrameworkCore;
using CheckAnalysis.Models;
using Microsoft.Data.SqlClient;

namespace CheckAnalysis.Data
{
    public class CheckDbContext : DbContext
    {
        public CheckDbContext(DbContextOptions<CheckDbContext> options)
    : base(options)
        {
            Database.EnsureCreated();
            Console.WriteLine(Database.EnsureCreated());
        }

        public DbSet<CheckData> CheckData { get; set; }
        public DbSet<ItemData> ItemData { get; set; }

    }
}
