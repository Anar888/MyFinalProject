using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Superadmin,admin")]
    public class ShippingController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public ShippingController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {


            var shippings = _context.Shippings.AsQueryable();

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = pageSizeStr == null ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Shipping>.Create(shippings, page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Shipping shipping)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Shippings.Add(shipping);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Shipping shipping = _context.Shippings.FirstOrDefault(x => x.Id == id);

            if (shipping == null) return NotFound();

            return View(shipping);
        }

        [HttpPost]
        public IActionResult Edit(Shipping shipping)
        {
            if (!ModelState.IsValid)
                return View();

            Shipping existship = _context.Shippings.FirstOrDefault(x => x.Id == shipping.Id);

            if (existship == null) return NotFound();
            existship.Title = shipping.Title;
            existship.Icon = shipping.Icon;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Shipping shipping = _context.Shippings.FirstOrDefault(x => x.Id == id);
            if (shipping == null) return NotFound();

            return View(shipping);
        }
        [HttpPost]
        public IActionResult Delete(Shipping shipping)
        {


            Shipping exstship = _context.Shippings.FirstOrDefault(x => x.Id == shipping.Id);

            if (exstship == null) return NotFound();

            _context.Shippings.Remove(exstship);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
