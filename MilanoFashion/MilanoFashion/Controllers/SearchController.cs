using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Controllers
{
    public class SearchController : Controller
    {
        private readonly DataContext _context;
        public SearchController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        public IActionResult Search(string search)
        {
            IEnumerable<Product> list = new List<Product>();
            if (search != null)
            {
                list = _context.Products.Include(p => p.ProductImages)
                .Where(t => t.Name.ToLower().Contains(search.ToLower())).Take(5);
                return PartialView("_SearchPartialView", list);
            }
            return Ok();
        }
    }
}
