using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Areas.Manage.ViewModels;
using MilanoFashion.Dal;
using MilanoFashion.Models;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("manage")]
    


    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(DataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Test()
        {
            //var result1 = await _roleManager.CreateAsync(new IdentityRole("Superadmin"));
            //var result2 = await _roleManager.CreateAsync(new IdentityRole("member"));


            AppUser admin = await _userManager.FindByNameAsync("SuperAdmin1");

            var result = await _userManager.AddToRoleAsync(admin, "Superadmin");

            return Ok();
        }

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new AppUser
            {
                FullName = "Admin2",
                UserName = "AdminUser2",
                Email = "admin111@gmail.com",
                IsAdmin = true,
            };
            var result = await _userManager.CreateAsync(appUser, "Admin123");
            return Ok();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser admin = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginVM.UserName && x.IsAdmin);

            if (admin == null)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(admin, loginVM.Password, loginVM.IsPersistent, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }

            return RedirectToAction("index", "dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account");
        }

        //public async Task<IActionResult> Test()
        //{
        //    AppUser superadmin = new AppUser
        //    {
        //        Email = "superadmin@mail.ru",
        //        UserName = "SuperAdmin1",
        //        FullName = "Anar Masiyev",
        //        IsAdmin = true
        //    };
        //    var result = await _userManager.CreateAsync(superadmin, "Admin123");
        //    return Ok(result.Errors);
        //}

        [Authorize(Roles = "Superadmin,admin")]
        public IActionResult Profile()
        {
            AppUser admin = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            AdminUpdateViewModel profileVM = new AdminUpdateViewModel
            {
              
                    FullName = admin.FullName,
                    Address = admin.Address,
                    City = admin.City,
                    Country = admin.Country,
                    Email = admin.Email,
                    Phone = admin.PhoneNumber,
                    UserName = admin.UserName
                
                
            };
            return View(profileVM);
        }


        [Authorize(Roles = "Superadmin,admin")]
        public IActionResult Edit(string username)
        {
            AppUser admin = _userManager.Users.FirstOrDefault(x => x.UserName == username);
            if (admin is null)
                return NotFound();

            AdminUpdateViewModel profileVM = new AdminUpdateViewModel
            {

                FullName = admin.FullName,
                Address = admin.Address,
                City = admin.City,
                Country = admin.Country,
                Email = admin.Email,
                Phone = admin.PhoneNumber,
                UserName = admin.UserName,
               


            };
            return View(profileVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminUpdateViewModel proVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);


            if (member.UserName != proVM.UserName && _userManager.Users.Any(x => x.NormalizedUserName == proVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "UserName has already taken");
                return View();
            }

            if (member.Email != proVM.Email && _userManager.Users.Any(x => x.NormalizedEmail == proVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email has already taken");
                return View();
            }

            member.Email = proVM.Email;
            member.FullName = proVM.FullName;
            member.PhoneNumber = proVM.Phone;
            member.Country = proVM.Country;
            member.City = proVM.City;
            member.Address = proVM.Address;
            member.UserName = proVM.UserName;

            var result = await _userManager.UpdateAsync(member);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return View();

                }
            }

            if (proVM.NewPassword != null)
            {
                if (string.IsNullOrWhiteSpace(proVM.OldPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword is required!");
                    return View();
                }

                if (!await _userManager.CheckPasswordAsync(member, proVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "OldPassword is incorrect!");
                    return View();
                }

                var passResult = _userManager.ChangePasswordAsync(member, proVM.OldPassword, proVM.NewPassword);

                if (!passResult.Result.Succeeded)
                {
                    foreach (var item in passResult.Result.Errors)
                    {
                        ModelState.AddModelError("NewPassword", item.Description);
                    }
                    return View();
                }
            }
            return RedirectToAction("profile");
        }


       

    }



}
