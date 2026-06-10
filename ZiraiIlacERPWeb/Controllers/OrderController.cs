using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZiraiIlacERPWeb.Data;
using ZiraiIlacERPWeb.Models;

namespace ZiraiIlacERPWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly ERPDbContext _context;

        public OrderController(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName");
            ViewBag.Products = await _context.Products.Where(p => p.StockQuantity > 0).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int customerId, List<OrderItemInput> items)
        {
            if (items == null || items.Count == 0)
            {
                ModelState.AddModelError("", "Siparişe en az bir ürün eklemelisiniz.");
                ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName");
                ViewBag.Products = await _context.Products.Where(p => p.StockQuantity > 0).ToListAsync();
                return View();
            }

            // Create Order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Generates order.Id

            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    // Check stock
                    int qty = Math.Min(item.Quantity, product.StockQuantity);
                    if (qty <= 0) continue;

                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = qty,
                        UnitPrice = product.Price
                    };

                    _context.OrderDetails.Add(detail);

                    // Update stock
                    product.StockQuantity -= qty;

                    totalAmount += qty * product.Price;
                }
            }

            order.TotalAmount = totalAmount;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Restore stocks
                if (order.OrderDetails != null)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(detail.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += detail.Quantity;
                        }
                    }
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class OrderItemInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
