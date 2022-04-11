using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Superadmin,admin")]
    public class AboutController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public AboutController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<About>.Create(_context.Abouts.AsQueryable(), page, pageSize));
        }

        public IActionResult Edit(int id)
        {
            About about = _context.Abouts.FirstOrDefault(x => x.Id == id);

            if (about == null) return NotFound();

            return View(about);
        }

        [HttpPost]
        public IActionResult Edit(About about)
        {
            if (!ModelState.IsValid)
                return View();

            About existabout = _context.Abouts.FirstOrDefault(x => x.Id == about.Id);

            if (existabout == null) return NotFound();
            existabout.Key = about.Key;
            existabout.Value = about.Value;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
