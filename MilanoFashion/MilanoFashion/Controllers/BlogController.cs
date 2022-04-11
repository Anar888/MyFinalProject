using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Controllers
{
    public class BlogController : Controller
    {
        private readonly DataContext _context;
        public BlogController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Blog> blogs = _context.Blogs.ToList();
            return View(blogs);
        }
        public IActionResult Detail(int id)
        {
            Blog blog = _context.Blogs.FirstOrDefault(x => x.Id == id);
            if (blog == null)
            {
                return RedirectToAction("error","home");
            }
           

            return View(blog);
        }
    }
}
