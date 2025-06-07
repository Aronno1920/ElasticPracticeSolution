using Microsoft.EntityFrameworkCore;
using SalesCore.Entities;

namespace SalesApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
    }
}
