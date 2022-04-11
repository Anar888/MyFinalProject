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


    public class CategoryController : Controller
    {

        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {


            var categories = _context.Categories.Include(x => x.Products).AsQueryable();

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = pageSizeStr == null ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Category>.Create(categories, page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View();

            Category existcat = _context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (existcat == null) return NotFound();
            existcat.Name = category.Name;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(Category category)
        {


            Category existcat = _context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (existcat == null) return NotFound();

            _context.Categories.Remove(existcat);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
