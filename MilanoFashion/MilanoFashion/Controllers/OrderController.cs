using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilanoFashion.Dal;
using MilanoFashion.Models;
using MilanoFashion.ViewModels;
using Newtonsoft.Json;

namespace MilanoFashion.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _context;

        public OrderController(UserManager<AppUser> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "member")]
        public IActionResult Checkout()
        {
            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            CheckOutViewModel checkoutVM = new CheckOutViewModel
            {
                Basket = _getBasket(member),
               
                
                
            };
            return View(checkoutVM);
        }

        private BasketViewModel _getBasket(AppUser member)
        {
            BasketViewModel basketVM = new BasketViewModel
            {
                Products = new List<ProductBasketItemViewModel>()
            };

            List<BasketItemViewModel> items = new List<BasketItemViewModel>();

            if (member == null)
            {
                string basketItemsStr = HttpContext.Request.Cookies["basket"];

                if (!string.IsNullOrWhiteSpace(basketItemsStr))
                    items = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
            }
            else
            {
                items = _context.BasketItems.Where(x => x.AppUserId == member.Id).Select(b => new BasketItemViewModel { ProductId = b.ProductId, Count = b.Count }).ToList();
            }

            foreach (var item in items)
            {
                Product product = _context.Products.FirstOrDefault(x => x.Id == item.ProductId);
                ProductBasketItemViewModel productItem = new ProductBasketItemViewModel
                {
                    Product = product,
                    Count = item.Count
                };

                decimal totalPrice = product.DiscountPercent > 0 ? (product.SalePrice * (1 - product.DiscountPercent / 100)) : product.SalePrice;
                basketVM.TotalPrice += totalPrice * item.Count;

                basketVM.Products.Add(productItem);
            }

            return basketVM;
        }
        [HttpPost]
        public IActionResult Create(Order order)
        {
            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            CheckOutViewModel checkoutVM = new CheckOutViewModel
            {
                Basket = _getBasket(member),
                Order = order
            };

            if (!ModelState.IsValid)
                return View("Checkout", checkoutVM);

            if (checkoutVM.Basket.Products.Count == 0)
            {
                ModelState.AddModelError("", "You must chose any product!");
                return View("Checkout", checkoutVM);
            }


            order.AppUserId = member?.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.ModifiedAt = DateTime.UtcNow.AddHours(4);
            order.Status = Enums.OrderStatus.Pending;

            order.OrderItems = new List<OrderItem>();

            foreach (var item in checkoutVM.Basket.Products)
            {
                OrderItem orderItem = new OrderItem
                {
                    ProductId = item.Product.Id,
                    SalePrice = item.Product.SalePrice,
                    CostPrice = item.Product.CostPrice,
                    DiscountedPrice = item.Product.DiscountPercent > 0 ? (item.Product.SalePrice * (1 - item.Product.DiscountPercent / 100)) : item.Product.SalePrice,
                    Count = item.Count
                };

                order.OrderItems.Add(orderItem);
                order.TotalPrice += orderItem.DiscountedPrice * orderItem.Count;
            }

            _context.Orders.Add(order);

            if (member == null)
                HttpContext.Response.Cookies.Delete("basket");
            else
                _context.BasketItems.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == member.Id));

            _context.SaveChanges();



            return RedirectToAction("profile", "account");
        }
    }
}
