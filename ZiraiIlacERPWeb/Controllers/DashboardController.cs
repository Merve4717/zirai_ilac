using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ZiraiIlacERPWeb.Data;

namespace ZiraiIlacERPWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ERPDbContext _context;

        public DashboardController(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Calculate metrics
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.TotalStock = await _context.Products.SumAsync(p => p.StockQuantity);
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount) ?? 0;

            // Critical stock (stock quantity <= 30)
            var criticalStock = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= 30)
                .OrderBy(p => p.StockQuantity)
                .Take(5)
                .ToListAsync();
            ViewBag.CriticalStock = criticalStock;

            // Recent orders (last 5)
            var recentOrders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();
            ViewBag.RecentOrders = recentOrders;

            // Category breakdown (Category Name, Product Count, Stock Count)
            var categoryStats = await _context.Categories
                .Select(c => new
                {
                    CategoryName = c.CategoryName,
                    ProductCount = c.Products.Count(),
                    TotalStock = c.Products.Sum(p => p.StockQuantity)
                }).ToListAsync();
            ViewBag.CategoryStats = categoryStats;

            return View();
        }
    }
}
