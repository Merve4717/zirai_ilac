using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ZiraiIlacERPWeb.Data;
using ZiraiIlacERPWeb.Models;

namespace ZiraiIlacERPWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ERPDbContext _context;

        public CategoryController(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
