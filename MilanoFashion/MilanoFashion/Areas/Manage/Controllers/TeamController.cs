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

    public class TeamController : Controller
    {

        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public TeamController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {

            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Team>.Create(_context.Teams.AsQueryable(), page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Team team)
        {
            if (team.ImageFile == null)
                ModelState.AddModelError("ImageFile", "Image file is required!");

            if (!ModelState.IsValid)
                return View();

            if (team.ImageFile.ContentType != "image/jpeg" && team.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "incorrect file type");
                return View();
            }

            if (team.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                return View();
            }
            string b = Guid.NewGuid().ToString() + team.ImageFile.FileName;
            if (b.Length > 99)
            {
                b.Substring(64, team.ImageFile.FileName.Length - 64);
            }

            team.Image = b;

            string path = Path.Combine(_env.WebRootPath, "uploads/teams", team.Image);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                team.ImageFile.CopyTo(stream);
            }


            _context.Teams.Add(team);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Team team = _context.Teams.FirstOrDefault(x => x.Id == id);
            if (team == null) return NotFound();



            return View(team);
        }
        [HttpPost]
        public IActionResult Delete(Team team)
        {


            Team existteam = _context.Teams.FirstOrDefault(x => x.Id == team.Id);

            if (existteam == null) return NotFound();

            if (existteam.Image != null)
            {
                string path = Path.Combine(_env.WebRootPath, "uploads/teams", existteam.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }


            _context.Teams.Remove(existteam);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Team team = _context.Teams.FirstOrDefault(x => x.Id == id);

            if (team == null) return NotFound();

            return View(team);
        }

        [HttpPost]
        public IActionResult Edit(Team team)
        {
            if (!ModelState.IsValid)
                return View();

            Team exsteam = _context.Teams.FirstOrDefault(x => x.Id == team.Id);
            if (exsteam == null) return NotFound();

            if (team.ImageFile != null)
            {
                if (team.ImageFile.ContentType != "image/jpeg" && team.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (team.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "file size must be less than 2mb");
                    return View();
                }

                team.Image = Guid.NewGuid().ToString() + team.ImageFile.FileName;

                //string path = _env.WebRootPath + @"uploads\sliders\" + slider.Image;
                string path = Path.Combine(_env.WebRootPath, "uploads/teams", team.Image);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    team.ImageFile.CopyTo(stream);
                }

                if (exsteam.Image != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/teams", exsteam.Image);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);
                }

                exsteam.Image = team.Image;
            }
            else
            {
                if (team.Image == null && exsteam.Image != null)
                {
                    string existPath = Path.Combine(_env.WebRootPath, "uploads/teams", exsteam.Image);
                    if (System.IO.File.Exists(existPath))
                        System.IO.File.Delete(existPath);

                    exsteam.Image = null;
                }
            }


            exsteam.Name = team.Name;
            exsteam.Position = team.Position;
            exsteam.Fburl = team.Fburl;
            exsteam.Twiturl = team.Twiturl;
            exsteam.Youtubeurl = team.Youtubeurl;
            exsteam.Googleurl = team.Googleurl;



            _context.SaveChanges();
            return RedirectToAction("index");
        }

    }
}
