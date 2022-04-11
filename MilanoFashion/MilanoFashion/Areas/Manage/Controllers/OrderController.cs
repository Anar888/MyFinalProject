using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Superadmin,admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _context;

        public OrderController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).AsQueryable();

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = pageSizeStr == null ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Order>.Create(orders, page, pageSize));
        }
        public IActionResult Edit(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            if (order.Id == 0)
            {
                ModelState.AddModelError("", "Xeta bas verdi!");
                return View();
            }

            Order existOrder = _context.Orders.FirstOrDefault(x => x.Id == order.Id);
            if (order == null) return NotFound();

            existOrder.Status = order.Status;
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
