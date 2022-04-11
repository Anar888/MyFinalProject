using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Models;
using MilanoFashion.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Services
{
    public class LayoutService
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(DataContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }
        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        public List<Brand> GetBrands()
        {
            return _context.Brands.Take(6).ToList();
        }
        public BasketViewModel GetBasket()
        {
            BasketViewModel basketVM = new BasketViewModel
            {
                Products = new List<ProductBasketItemViewModel>(),
                TotalPrice = 0
            };

            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            AppUser member = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                var baksetStr = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

                if (!string.IsNullOrWhiteSpace(baksetStr))
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(baksetStr);

            }
            else
            {
                basketItems = _context.BasketItems.Where(x => x.AppUserId == member.Id).Select(b => new BasketItemViewModel { ProductId = b.ProductId, Count = b.Count }).ToList();
            }


            foreach (var item in basketItems)
            {
                Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                ProductBasketItemViewModel productBasketitems = new ProductBasketItemViewModel
                {
                    Product = product,
                    Count = item.Count
                };

                basketVM.Products.Add(productBasketitems);
                decimal totalPrice = product.DiscountPercent > 0 ? (product.SalePrice * (1 - product.DiscountPercent / 100)) : product.SalePrice;
                basketVM.TotalPrice += totalPrice * item.Count;
            }

            return basketVM;
        }
        public WishlistViewModel GetWishlist()
        {
            WishlistViewModel basketVM = new WishlistViewModel
            {
                Products = new List<ProductWishlistItemViewModel>(),
            };

            List<WishlistItemViewModel> wishlistItems = new List<WishlistItemViewModel>();
            AppUser member = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                var baksetStr = _httpContextAccessor.HttpContext.Request.Cookies["wishlist"];

                if (!string.IsNullOrWhiteSpace(baksetStr))
                    wishlistItems = JsonConvert.DeserializeObject<List<WishlistItemViewModel>>(baksetStr);

            }
            else
            {
                wishlistItems = _context.WishlistItems.Where(x => x.AppUserId == member.Id).Select(b => new WishlistItemViewModel { ProductId = b.ProductId, }).ToList();
            }


            foreach (var item in wishlistItems)
            {
                Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                ProductWishlistItemViewModel productBasketitems = new ProductWishlistItemViewModel
                {
                    Product = product,
                };

                basketVM.Products.Add(productBasketitems);
               
            }

            return basketVM;
        }


    }
}
