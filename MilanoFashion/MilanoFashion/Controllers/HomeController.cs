using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.ViewModels;
using MilanoFashion.Models;

namespace MilanoFashion.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        public HomeController(DataContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            HomeViewModel homeVM = new HomeViewModel
            {
                Sliders = _context.Sliders.ToList(),
                Products = _context.Products.Include(x => x.ProductImages).Take(15).ToList(),
                FoundersInfoSliders = _context.FoundersInfoSliders.ToList(),
                Shippings = _context.Shippings.ToList(),
                Banners = _context.Banners.ToList(),
                Blogs = _context.Blogs.ToList(),
                BrandLogos = _context.BrandLogos.ToList(),
                News = _context.Products.Include(x => x.ProductImages).Where(x => x.IsNew == true && x.IsAvailable == true).Take(15).ToList(),
                Featured = _context.Products.Include(x => x.ProductImages).Where(x => x.IsFeatured == true && x.IsAvailable == true).Take(15).ToList(),
                Special = _context.Products.Include(x => x.ProductImages).Where(x => x.IsSpecial == true && x.IsAvailable == true).Take(15).ToList(),
            };
            return View(homeVM);
        }
        public IActionResult AboutUs()
        {
            AboutViewModel aboutVm = new AboutViewModel
            {
                Abouts = _context.Abouts.ToDictionary(x => x.Key, x => x.Value),
                Teams = _context.Teams.ToList()
                
            };
            return View(aboutVm);
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactMessage msg)
        {


            if (!ModelState.IsValid)
                return View();


            _context.ContactMessages.Add(msg);
            _context.SaveChanges();

            return RedirectToAction("index");

        }
        public IActionResult Faqs()
        {
            List<Faqs> faq = _context.Faqss.ToList();
            return View(faq);
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
