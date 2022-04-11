using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Models;
using MilanoFashion.ViewModels;
using Newtonsoft.Json;

namespace MilanoFashion.Controllers
{
    public class ShopController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShopController(DataContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public IActionResult GetProduct(int id)
        {

            Product product = _context.Products.Include(x => x.Brand).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x => x.Color).Include(x => x.ProductImages).Include(x => x.ProductSizes).ThenInclude(x => x.Size).Include(x=>x.ProductComments).FirstOrDefault(x => x.Id == id);

            return PartialView("_ProductModalPartial", product);
        }
        public IActionResult Index(int? brandid, decimal? minPrice, decimal? maxPrice, int? categoryid, int page = 1)
        {
            var products = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x => x.Color).Where(x => x.IsAvailable);
            ViewBag.BrandId = brandid;
            ViewBag.CategoryId = categoryid;
            ViewBag.PageIndex = page;

            ProductViewModel prviewm = new ProductViewModel();

            if (brandid != null)
                products = products.Where(x => x.BrandId == brandid);
            if (categoryid != null)
                products = products.Where(x => x.CategoryId == categoryid);
           


            if (products.Any())
            {
                prviewm.MinPrice = products.Min(x => x.SalePrice);
                prviewm.MaxPrice = products.Max(x => x.SalePrice);
            }

            ViewBag.FilterMinPrice = minPrice ?? prviewm.MinPrice;
            ViewBag.FilterMaxPrice = maxPrice ?? prviewm.MaxPrice;

            if (minPrice != null && maxPrice != null)
                products = products.Where(x => x.SalePrice >= minPrice && x.SalePrice <= maxPrice);
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 2d);

            
            prviewm.Brands = _context.Brands.Include(x => x.Products).ToList();
            prviewm.Categories = _context.Categories.Include(x => x.Products).ToList();
            prviewm.Products = products.Skip((page - 1) * 2).Take(3).ToList();

            return View(prviewm);
        }
        public IActionResult Detail(int id)
        {
            Product product = _context.Products.Include(x => x.Brand).Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x => x.Color).Include(x => x.ProductImages).Include(x => x.ProductSizes).ThenInclude(x => x.Size).Include(x => x.ProductComments).ThenInclude(c => c.AppUser).FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return RedirectToAction("error", "home");
            }
            ProductDetailViewModel prodvm = new ProductDetailViewModel
            {
                Product = product,
                Comment = new ProductComment() { ProductId=id},
                RelatedProducts = _context.Products.Include(x => x.ProductImages).Where(x => x.CategoryId == product.CategoryId).Take(3).ToList()
            };

            return View(prodvm);
        }
        [HttpPost]
        public IActionResult Comment(ProductComment comment)
        {
            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
                return RedirectToAction("login", "account");

            Product product = _context.Products
             .Include(x => x.ProductImages).Include(x => x.Category).Include(x => x.Brand)
             .Include(x => x.ProductColors).ThenInclude(x => x.Color)
             .Include(x => x.ProductComments).ThenInclude(c => c.AppUser)
             .FirstOrDefault(x => x.Id == comment.ProductId);

            if (product == null)
                return RedirectToAction("error", "home");


            if (!ModelState.IsValid)
            {


                ProductDetailViewModel productvm = new ProductDetailViewModel
                {
                    Product = product,
                    Comment = comment,
                    RelatedProducts = _context.Products.Include(x => x.ProductImages).Where(x => x.BrandId == product.BrandId).Take(4).ToList()
                };

                return View("Detail", productvm);
            }

            comment.AppUserId = member.Id;
            comment.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.ProductComments.Add(comment);
            product.Rate = (int)Math.Ceiling(product.ProductComments.Sum(x => x.Rate) / (double)product.ProductComments.Count);

            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = comment.ProductId });
        }


        public IActionResult AddBasket(int id)
        {
            if (!_context.Products.Any(x => x.Id == id))
                return RedirectToAction("error", "home");

            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                string basketItemsStr = HttpContext.Request.Cookies["basket"];
                List<BasketItemViewModel> items = new List<BasketItemViewModel>();

                if (!string.IsNullOrWhiteSpace(basketItemsStr))
                    items = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                BasketItemViewModel item = items.FirstOrDefault(x => x.ProductId == id);

                if (item == null)
                {
                    item = new BasketItemViewModel { ProductId = id, Count = 1 };
                    items.Add(item);
                }
                else
                    item.Count++;

                basketItemsStr = JsonConvert.SerializeObject(items);

                HttpContext.Response.Cookies.Append("basket", basketItemsStr);

                return PartialView("_BasketPartialView", _getBasket(items));
            }
            else
            {
                BasketItem item = _context.BasketItems.FirstOrDefault(x => x.AppUserId == member.Id && x.ProductId == id);

                if (item == null)
                {
                    item = new BasketItem
                    {
                        AppUserId = member.Id,
                        ProductId = id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        Count = 1
                    };
                    _context.BasketItems.Add(item);
                }
                else
                    item.Count++;

                _context.SaveChanges();

                var items = _context.BasketItems.Where(x => x.AppUserId == member.Id).ToList();
                return PartialView("_BasketPartialView", _getBasket(items));
            }
        }

        private BasketViewModel _getBasket(List<BasketItemViewModel> basketItems)
        {
            BasketViewModel basketVM = new BasketViewModel
            {
                Products = new List<ProductBasketItemViewModel>(),
                TotalPrice = 0
            };

            foreach (var item in basketItems)
            {
                Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                ProductBasketItemViewModel productBasketItem = new ProductBasketItemViewModel
                {
                    Product = product,
                    Count = item.Count
                };

                basketVM.Products.Add(productBasketItem);
                decimal totalPrice = product.DiscountPercent > 0 ? (product.SalePrice * (1 - product.DiscountPercent / 100)) : product.SalePrice;
                basketVM.TotalPrice += totalPrice * item.Count;
            }

            return basketVM;
        }

        private BasketViewModel _getBasket(List<BasketItem> basketItems)
        {
            BasketViewModel basketVM = new BasketViewModel
            {
                Products = new List<ProductBasketItemViewModel>(),
                TotalPrice = 0
            };

            foreach (var item in basketItems)
            {
                Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                ProductBasketItemViewModel productBasketItem = new ProductBasketItemViewModel
                {
                    Product = product,
                    Count = item.Count
                };

                basketVM.Products.Add(productBasketItem);
                decimal totalPrice = product.DiscountPercent > 0 ? (product.SalePrice * (1 - product.DiscountPercent / 100)) : product.SalePrice;
                basketVM.TotalPrice += totalPrice * item.Count;
            }

            return basketVM;
        }
        public IActionResult RemoveBasket(int id)
        {
            if (!_context.Products.Any(x => x.Id == id))
                return RedirectToAction("error", "home");

            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                string basketItemsStr = HttpContext.Request.Cookies["basket"];
                List<BasketItemViewModel> items = new List<BasketItemViewModel>();

                if (!string.IsNullOrWhiteSpace(basketItemsStr))
                    items = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                BasketItemViewModel item = items.FirstOrDefault(x => x.ProductId == id);

                if (item.Count==1)
                {
                    items.Remove(item);
                    
                }
                else 
                {
                    item.Count--;
                }
                
        

                basketItemsStr = JsonConvert.SerializeObject(items);

                HttpContext.Response.Cookies.Append("basket", basketItemsStr);

                return PartialView("_BasketPartialView", _getBasket(items));
            }
            else
            {
                BasketItem item = _context.BasketItems.FirstOrDefault(x => x.AppUserId == member.Id && x.ProductId == id);

                if (item.Count==1)
                {
                  
                    _context.BasketItems.Remove(item);
                }
                else
                {
                    item.Count--;

                }

                _context.SaveChanges();

                var items = _context.BasketItems.Where(x => x.AppUserId == member.Id).ToList();
                return PartialView("_BasketPartialView", _getBasket(items));
            }
        }

        public IActionResult WishList()
        {
            WishlistViewModel basketVM = new WishlistViewModel
            {
                Products = new List<ProductWishlistItemViewModel>(),
            };

            List<WishlistItemViewModel> wishlistItems = new List<WishlistItemViewModel>();
            AppUser member = null;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                var baksetStr = HttpContext.Request.Cookies["wishlist"];

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

           
            return View(basketVM);
        }

        public IActionResult AddWishlist(int id)
        {
            if (!_context.Products.Any(x => x.Id == id))
                return RedirectToAction("error", "home");

            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                string wishlistItemsStr = HttpContext.Request.Cookies["wishlist"];
                List<WishlistItemViewModel> items = new List<WishlistItemViewModel>();

                if (!string.IsNullOrWhiteSpace(wishlistItemsStr))
                    items = JsonConvert.DeserializeObject<List<WishlistItemViewModel>>(wishlistItemsStr);

                WishlistItemViewModel item = items.FirstOrDefault(x => x.ProductId == id);

                if (item == null)
                {
                    item = new WishlistItemViewModel { ProductId = id,  };
                    items.Add(item);
                }
                

                wishlistItemsStr = JsonConvert.SerializeObject(items);

                HttpContext.Response.Cookies.Append("wishlist", wishlistItemsStr);

                return PartialView("_WishlistPartialView", _getWishlist(items));
            }
            else
            {
                WishlistItem item = _context.WishlistItems.FirstOrDefault(x => x.AppUserId == member.Id && x.ProductId == id);

                if (item == null)
                {
                    item = new WishlistItem
                    {
                        AppUserId = member.Id,
                        ProductId = id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                    };
                    _context.WishlistItems.Add(item);
                }

                _context.SaveChanges();

                var items = _context.WishlistItems.Where(x => x.AppUserId == member.Id).ToList();
                return PartialView("_WishlistPartialView", _getWishlist(items));


            }
        }
        private WishlistViewModel _getWishlist(List<WishlistItemViewModel> wishlistItems)
        {
            WishlistViewModel wishlistVm = new WishlistViewModel
            {
                Products = new List<ProductWishlistItemViewModel>(),
            };

            foreach (var item in wishlistItems)
            {
                Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                ProductWishlistItemViewModel productBasketItem = new ProductWishlistItemViewModel
                {
                    Product = product,
                };

                wishlistVm.Products.Add(productBasketItem);
              
            }

            return wishlistVm;
        }
        private WishlistViewModel _getWishlist(List<WishlistItem> wishlistItems)
        {
            WishlistViewModel wishlistVM = new WishlistViewModel
            {
                Products = new List<ProductWishlistItemViewModel>(),
            };

            foreach (var item in wishlistItems)
            {
                Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                ProductWishlistItemViewModel productWishlistItem = new ProductWishlistItemViewModel
                {
                    Product = product,
                };

                wishlistVM.Products.Add(productWishlistItem
);
                
            }

            return wishlistVM;
        }
        public IActionResult RemoveWishlist(int id)
        {
            if (!_context.Products.Any(x => x.Id == id))
                return RedirectToAction("error", "home");

            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }

            if (member == null)
            {
                string wishlistItemsStr = HttpContext.Request.Cookies["wishlist"];
                List<WishlistItemViewModel> items = new List<WishlistItemViewModel>();

                if (!string.IsNullOrWhiteSpace(wishlistItemsStr))
                    items = JsonConvert.DeserializeObject<List<WishlistItemViewModel>>(wishlistItemsStr);

                WishlistItemViewModel item = items.FirstOrDefault(x => x.ProductId == id);

               
                    items.Remove(item);






                wishlistItemsStr = JsonConvert.SerializeObject(items);

                HttpContext.Response.Cookies.Append("wishlist", wishlistItemsStr);

                return PartialView("_WishlistPartialView", _getWishlist(items));
            }
            else
            {
                WishlistItem item = _context.WishlistItems.FirstOrDefault(x => x.AppUserId == member.Id && x.ProductId == id);

            
                

                    _context.WishlistItems.Remove(item);
                
               

                _context.SaveChanges();

                var items = _context.WishlistItems.Where(x => x.AppUserId == member.Id).ToList();
                return PartialView("_WishlistPartialView", _getWishlist(items));
            }
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
                Product product = _context.Products.Include(x=>x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
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
        public IActionResult Cart()
        {
            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }
            return View(_getBasket(member));
        }
    }
}
