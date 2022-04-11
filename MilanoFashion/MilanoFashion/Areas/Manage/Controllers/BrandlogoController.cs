using System;
using System.Collections.Generic;
using System.IO;
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


    public class BrandlogoController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public BrandlogoController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<BrandLogo>.Create(_context.BrandLogos.AsQueryable(), page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(BrandLogo brandlogo)
        {
            if (brandlogo.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (brandlogo.ImageFile.ContentType != "image/jpeg" && brandlogo.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (brandlogo.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            string b = Guid.NewGuid().ToString() + brandlogo.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, brandlogo.ImageFile.FileName.Length - 64);
            }

            brandlogo.ImageUrl = b;

            string path = Path.Combine(_env.WebRootPath, "uploads/brandlogos", brandlogo.ImageUrl);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                brandlogo.ImageFile.CopyTo(stream);
            }


            _context.BrandLogos.Add(brandlogo);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            BrandLogo brandlogo = _context.BrandLogos.FirstOrDefault(x => x.Id == id);
            if (brandlogo == null) return NotFound();



            return View(brandlogo);
        }
        [HttpPost]
        public IActionResult Delete(BrandLogo brandlogo)
        {



            BrandLogo existbrandlogo = _context.BrandLogos.FirstOrDefault(x => x.Id == brandlogo.Id);

            if (existbrandlogo == null) return NotFound();

            if (existbrandlogo.ImageUrl != null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/brandlogos", existbrandlogo.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            _context.BrandLogos.Remove(existbrandlogo);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            BrandLogo brandlogo = _context.BrandLogos.FirstOrDefault(x => x.Id == id);

            if (brandlogo == null) return NotFound();

            return View(brandlogo);
        }

        [HttpPost]
        public IActionResult Edit(BrandLogo brandLogo)
        {
            if (!ModelState.IsValid)
                return View();

            BrandLogo existbrandlogo = _context.BrandLogos.FirstOrDefault(x => x.Id == brandLogo.Id);
            if (existbrandlogo == null) return NotFound();

            if (brandLogo.ImageFile != null)
            {
                if (brandLogo.ImageFile.ContentType != "image/jpeg" && brandLogo.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (brandLogo.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                brandLogo.ImageUrl = Guid.NewGuid().ToString() + brandLogo.ImageFile.FileName;

                string path = Path.Combine(_env.WebRootPath, "uploads/brandlogos", brandLogo.ImageUrl);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    brandLogo.ImageFile.CopyTo(stream);
                }

                if (existbrandlogo.ImageUrl != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/brandlogos", existbrandlogo.ImageUrl);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                existbrandlogo.ImageUrl = brandLogo.ImageUrl;
            }
            else
            {
                if (brandLogo.ImageUrl == null && existbrandlogo.ImageUrl != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/brandlogos", existbrandlogo.ImageUrl);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    existbrandlogo.ImageUrl = null;
                }
            }



            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
