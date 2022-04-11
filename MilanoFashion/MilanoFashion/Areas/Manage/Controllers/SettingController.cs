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
    public class SettingController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Setting>.Create(_context.Settings.AsQueryable(), page, pageSize));
        }

        public IActionResult Edit(int id)
        {
            Setting setting = _context.Settings.FirstOrDefault(x => x.Id == id);

            if (setting == null) return NotFound();

            return View(setting);
        }

        [HttpPost]
        public IActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid)
                return View();

            Setting existsetting = _context.Settings.FirstOrDefault(x => x.Id == setting.Id);

            if (existsetting == null) return NotFound();
            existsetting.Key = setting.Key;
            existsetting.Value = setting.Value;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
