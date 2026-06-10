using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZiraiIlacERPWeb.Data;
using ZiraiIlacERPWeb.Models;

namespace ZiraiIlacERPWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ERPDbContext _context;

        public ProductController(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}