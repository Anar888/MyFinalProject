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


    public class ColorController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public ColorController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {

            var colors = _context.Colors.Include(x => x.ProductColors).ThenInclude(x => x.Product).AsQueryable();

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = pageSizeStr == null ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Color>.Create(colors, page, pageSize));


        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Colors.Add(color);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Color color = _context.Colors.FirstOrDefault(x => x.Id == id);

            if (color == null) return NotFound();

            return View(color);
        }

        [HttpPost]
        public IActionResult Edit(Color color)
        {
            if (!ModelState.IsValid)
                return View();

            Color exstcolor = _context.Colors.FirstOrDefault(x => x.Id == color.Id);

            if (exstcolor == null) return NotFound();
            exstcolor.Name = color.Name;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Color color = _context.Colors.FirstOrDefault(x => x.Id == id);
            if (color == null) return NotFound();

            return View(color);
        }
        [HttpPost]
        public IActionResult Delete(Color color)
        {


            Color existcolor = _context.Colors.FirstOrDefault(x => x.Id == color.Id);

            if (existcolor == null) return NotFound();

            _context.Colors.Remove(existcolor);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
