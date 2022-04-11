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

    public class FoundersInfoSliderController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public FoundersInfoSliderController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<FoundersInfoSlider>.Create(_context.FoundersInfoSliders.AsQueryable(), page, pageSize));


        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(FoundersInfoSlider slider)
        {
            if (slider.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (slider.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            string b = Guid.NewGuid().ToString() + slider.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, slider.ImageFile.FileName.Length - 64);
            }

            slider.Image = b;

            string path = Path.Combine(_env.WebRootPath, "uploads/FoundersInfoSliders", slider.Image);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                slider.ImageFile.CopyTo(stream);
            }


            _context.FoundersInfoSliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            FoundersInfoSlider slider = _context.FoundersInfoSliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();



            return View(slider);
        }
        [HttpPost]
        public IActionResult Delete(FoundersInfoSlider slider)
        {


            FoundersInfoSlider existslider = _context.FoundersInfoSliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existslider == null) return NotFound();

            if (existslider.Image != null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/FoundersInfoSliders", existslider.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            _context.FoundersInfoSliders.Remove(existslider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            FoundersInfoSlider slider = _context.FoundersInfoSliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        public IActionResult Edit(FoundersInfoSlider slider)
        {
            if (!ModelState.IsValid)
                return View();

            FoundersInfoSlider existslider = _context.FoundersInfoSliders.FirstOrDefault(x => x.Id == slider.Id);
            if (existslider == null) return NotFound();

            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                slider.Image = Guid.NewGuid().ToString() + slider.ImageFile.FileName;

                //string path = _env.WebRootPath + @"uploads\sliders\" + slider.Image;
                string path = Path.Combine(_env.WebRootPath, "uploads/FoundersInfoSliders", slider.Image);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.ImageFile.CopyTo(stream);
                }

                if (existslider.Image != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/FoundersInfoSliders", existslider.Image);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                existslider.Image = slider.Image;
            }
            else
            {
                if (slider.Image == null && existslider.Image != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/FoundersInfoSliders", existslider.Image);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    existslider.Image = null;
                }
            }


            existslider.Name = slider.Name;
            existslider.Bio = slider.Bio;
            existslider.Position = slider.Position;
            


            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
