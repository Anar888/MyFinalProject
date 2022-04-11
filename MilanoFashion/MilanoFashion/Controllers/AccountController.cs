using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Models;
using MilanoFashion.Utils;
using MilanoFashion.ViewModels;

namespace MilanoFashion.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(DataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser member = await _userManager.FindByNameAsync(registerVM.UserName);
            if (member != null)
            {
                ModelState.AddModelError("UserName", "UserName alredy exist");
                return View();
            }

            member = new AppUser
            {
                FullName = registerVM.FullName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(member, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }

            var roleResult = await _userManager.AddToRoleAsync(member, "member");

            if (!roleResult.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }

            await _signInManager.SignInAsync(member, true);

            return RedirectToAction("index", "home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser member = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginVM.UserName && !x.IsAdmin);

            if (member == null)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(member, loginVM.Password, loginVM.IsPersistent, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }

            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest();

            var dbUser = await _userManager.FindByEmailAsync(email);
            if (dbUser is null)
                return RedirectToAction("error", "home");

          

            var token = await _userManager.GeneratePasswordResetTokenAsync(dbUser);

            var link = Url.Action("ResetPassword", "Account", new { dbUser.Id, token }, protocol: HttpContext.Request.Scheme);

            var message = $"<a href='{link}'>reset password</a>";

            await EmailUtil.SendEmailAsync(email, "Reset Password", message);

            return RedirectToAction("Login");
        }
        public async Task<IActionResult> ResetPassword(string id, string token)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
                return BadRequest();

            var dbUser = await _userManager.FindByIdAsync(id);
            if (dbUser is null)
                return RedirectToAction("error", "home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string token, ResetPasswordViewModel resetPasswordVm)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var dbUser = await _userManager.FindByIdAsync(id);
            if (dbUser is null)
                return RedirectToAction("error", "home");

            var result = await _userManager.ResetPasswordAsync(dbUser, token, resetPasswordVm.NewPassword);
            if (result.Errors == null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return RedirectToAction("Login");
        }
        [Authorize(Roles = "member")]
        public IActionResult Profile()
        {
            AppUser member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            if (member==null)
            {
                return Redirect("login");
            }

            MemberProfileViewModel profileVM = new MemberProfileViewModel
            {
                Member = new MemberUpdateViewModel
                {
                    FullName = member.FullName,
                    Address = member.Address,
                    City = member.City,
                    Country = member.Country,
                    Email = member.Email,
                    Phone = member.PhoneNumber,
                    UserName = member.UserName
                },
                Orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).Where(x => x.AppUserId == member.Id).ToList()
            };
            return View(profileVM);
        }
        [Authorize(Roles = "member")]
        [HttpPost]
        public async Task<IActionResult> Edit(MemberUpdateViewModel memberVM)
        {
            MemberProfileViewModel profileVM = new MemberProfileViewModel
            {
                Member = memberVM,
                Orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).Where(x => x.AppUser.UserName == memberVM.UserName).ToList()

            };

            TempData["ProfileTab"] = "Account";

            if (!ModelState.IsValid)
                return View("Profile", profileVM);

            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);

            if (member.UserName != memberVM.UserName && _userManager.Users.Any(x => x.NormalizedUserName == memberVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "UserName has already taken");
                return View("Profile", profileVM);
            }

            if (member.Email != memberVM.Email && _userManager.Users.Any(x => x.NormalizedEmail == memberVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email has already taken");
                return View("Profile", profileVM);
            }

            member.Email = memberVM.Email;
            member.FullName = memberVM.FullName;
            member.UserName = memberVM.UserName;
            member.PhoneNumber = memberVM.Phone;
            member.Country = memberVM.Country;
            member.City = memberVM.City;
            member.Address = memberVM.Address;

            var result = await _userManager.UpdateAsync(member);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View("Profile", profileVM);
            }

            if (memberVM.Password != null)
            {
                if (string.IsNullOrWhiteSpace(memberVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword is required!");
                    return View("Profile", profileVM);
                }

                if (!await _userManager.CheckPasswordAsync(member, memberVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword is incorrect!");
                    return View("Profile", profileVM);
                }

                var passResult = _userManager.ChangePasswordAsync(member, memberVM.CurrentPassword, memberVM.Password);

                if (!passResult.Result.Succeeded)
                {
                    foreach (var item in passResult.Result.Errors)
                    {
                        ModelState.AddModelError("Password", item.Description);
                    }
                    return View("Profile", profileVM);
                }

            }

            return RedirectToAction("profile");
        }


    }
}
