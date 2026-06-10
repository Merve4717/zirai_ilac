using Microsoft.EntityFrameworkCore;
using ZiraiIlacERPAPI.Models;

namespace ZiraiIlacERPAPI.Data
{
    public class ERPDbContext : DbContext
    {
        public ERPDbContext(DbContextOptions<ERPDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
