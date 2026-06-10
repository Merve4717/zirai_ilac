using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ZiraiIlacERPWeb.Models;

namespace ZiraiIlacERPWeb.Data
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
