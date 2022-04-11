using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilanoFashion.Areas.Manage.ViewModels;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Superadmin")]
    public class AdminController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(DataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AdminRegisterViewModel adminVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser member = await _userManager.FindByNameAsync(adminVm.UserName);
            if (member != null)
            {
                ModelState.AddModelError("Username", "UserName is already exist");
                return View();
            }
            member = new AppUser
            {
                FullName = adminVm.FullName,
                UserName = adminVm.UserName,
                Email = adminVm.Email,
                IsAdmin = true
            };
            var result = await _userManager.CreateAsync(member, adminVm.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
            return RedirectToAction("RoleToAdmin", member);
        }
        public async Task<IActionResult> RoleToAdmin(AppUser member)
        {
            var admin = _context.AppUsers.FirstOrDefault(x => x.UserName == member.UserName);
            //var result1 = await _roleManager.CreateAsync(new IdentityRole("Admin"));
            AppUser adminrole = await _userManager.FindByNameAsync(admin.UserName);
            await _userManager.AddToRoleAsync(adminrole, "admin");
            return RedirectToAction("index", "dashboard");
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<AppUser>.Create(_context.AppUsers.Where(x => x.IsAdmin == true).AsQueryable(), page, pageSize));
        }
        public IActionResult Edit(string id)
        {
            AppUser admin = _context.AppUsers
               .Where(x => x.IsAdmin == true).FirstOrDefault(x => x.Id == id);

            if (admin == null)
            {
                return NotFound();
            }
            SuperUpdateViewModel adminVm = new SuperUpdateViewModel
            {
                FullName = admin.FullName,
                Email = admin.Email,
                Username = admin.UserName
            };




            return View(adminVm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SuperUpdateViewModel proVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser member = await _userManager.FindByNameAsync(proVM.Username);

            if (member.Email != proVM.Email && _userManager.Users.Any(x => x.NormalizedEmail == proVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email has already taken");
                return View();
            }

            member.Email = proVM.Email;
            member.FullName = proVM.FullName;


            if (proVM.Password != null)
            {
                var passResult = _userManager.RemovePasswordAsync(member);

                if (passResult.Result.Succeeded)
                {
                    passResult = _userManager.AddPasswordAsync(member, proVM.Password);
                    if (passResult.Result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {

                    foreach (var item in passResult.Result.Errors)
                    {
                        ModelState.AddModelError("Password", item.Description);
                    }
                    return View();
                }
            }
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(string id)
        {
            AppUser admin = _context.AppUsers
               .Where(x => x.IsAdmin == true).FirstOrDefault(x => x.Id == id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(AppUser user)
        {

            AppUser existuser = _context.AppUsers.FirstOrDefault(x => x.Id == user.Id);

            if (existuser == null) return NotFound();

            


            _context.AppUsers.Remove(existuser);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


    }
}
