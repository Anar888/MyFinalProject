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
    public class SizeController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public SizeController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {

            var sizes = _context.Sizes.Include(x => x.ProductSizes).ThenInclude(x => x.Product).AsQueryable();

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = pageSizeStr == null ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Size>.Create(sizes, page, pageSize));


        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Sizes.Add(size);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Size size = _context.Sizes.FirstOrDefault(x => x.Id == id);

            if (size == null) return NotFound();

            return View(size);
        }

        [HttpPost]
        public IActionResult Edit(Size size)
        {
            if (!ModelState.IsValid)
                return View();

            Size existsize = _context.Sizes.FirstOrDefault(x => x.Id == size.Id);

            if (existsize == null) return NotFound();
            existsize.Name = size.Name;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Size size = _context.Sizes.FirstOrDefault(x => x.Id == id);
            if (size == null) return NotFound();

            return View(size);
        }
        [HttpPost]
        public IActionResult Delete(Size size)
        {


            Size existsize = _context.Sizes.FirstOrDefault(x => x.Id == size.Id);

            if (existsize == null) return NotFound();

            _context.Sizes.Remove(existsize);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
