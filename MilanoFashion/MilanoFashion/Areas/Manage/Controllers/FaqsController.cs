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
    public class FaqsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public FaqsController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Faqs>.Create(_context.Faqss.AsQueryable(), page, pageSize));
        }

        public IActionResult Edit(int id)
        {
            Faqs faq = _context.Faqss.FirstOrDefault(x => x.Id == id);

            if (faq == null) return NotFound();

            return View(faq);
        }

        [HttpPost]
        public IActionResult Edit(Faqs faq)
        {
            if (!ModelState.IsValid)
                return View();

            Faqs existfaq = _context.Faqss.FirstOrDefault(x => x.Id == faq.Id);

            if (existfaq == null) return NotFound();
            existfaq.AccTitle = faq.AccTitle;
            existfaq.AccDesc = faq.AccDesc;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Faqs faq)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Faqss.Add(faq);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Faqs faq = _context.Faqss.FirstOrDefault(x => x.Id == id);
            if (faq == null) return NotFound();

            return View(faq);
        }
        [HttpPost]
        public IActionResult Delete(Faqs faqs)
        {


            Faqs extfaq = _context.Faqss.FirstOrDefault(x => x.Id == faqs.Id);

            if (extfaq == null) return NotFound();

            _context.Faqss.Remove(extfaq);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
