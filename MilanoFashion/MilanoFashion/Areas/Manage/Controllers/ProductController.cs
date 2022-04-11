using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Dal;
using MilanoFashion.Helpers;
using MilanoFashion.Models;

namespace MilanoFashion.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Superadmin,admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            string pageSizeStr = _context.Settings.FirstOrDefault(x => x.Key == "PageSizeValue").Value;
            int pageSize = string.IsNullOrWhiteSpace(pageSizeStr) ? 3 : int.Parse(pageSizeStr);
            return View(PagenatedList<Product>.Create(_context.Products.Include(x => x.ProductImages).AsQueryable(), page, pageSize));


        }
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Sizes = _context.Sizes.Include(x => x.ProductSizes).ThenInclude(x => x.Product).ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Sizes = _context.Sizes.Include(x => x.ProductSizes).ThenInclude(x => x.Product).ToList();


            if (product.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "Invalid PosterFile");
                return View();
            }
            else
            {
                if (product.PosterFile.ContentType != "image/jpeg" && product.PosterFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterFile", "file type must be image/jpeg or image/png");
                    return View();
                }

                if (product.PosterFile.Length > 2097152)
                {
                    ModelState.AddModelError("PosterFile", "file size must be less than 2mb");
                    return View();
                }
            }

            if (product.Files != null)
            {
                foreach (var photo in product.Files)
                {
                    if (photo.ContentType != "image/jpeg" && photo.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Files", "file type must be image/jpeg or image/png");
                        return View();
                    }

                    if (photo.Length > 2097152)
                    {
                        ModelState.AddModelError("Files", "file size must be less than 2mb");
                        return View();
                    }
                }

            }
            if (!_context.Brands.Any(x => x.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Invalid BrandId ");
                return View();
            }
            if (!_context.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Invalid CategoryId ");
                return View();
            }

            product.ProductImages = new List<ProductImage>();
            ProductImage posterimage = new ProductImage
            {
                PosterStatus = true,
                Image = FileManager.Save(_env.WebRootPath, "uploads/products", product.PosterFile)
            };
            product.ProductImages.Add(posterimage);

            if (product.Files != null)
            {
                foreach (var photo in product.Files)
                {
                    ProductImage images = new ProductImage
                    {
                        Image = FileManager.Save(_env.WebRootPath, "uploads/products", photo)
                    };
                    product.ProductImages.Add(images);
                }
            }

            product.ProductColors = new List<ProductColor>();
            if (product.ColorIds != null)
            {
                foreach (var colorid in product.ColorIds)
                {
                    if (_context.Colors.Any(x => x.Id == colorid))
                    {
                        ProductColor color = new ProductColor { ColorId = colorid };
                        product.ProductColors.Add(color);
                    }
                    else
                    {
                        ModelState.AddModelError("ColorIds", "Color not found");
                        return View();
                    }


                }
            }
            product.ProductSizes = new List<ProductSize>();
            if (product.SizeIds != null)
            {
                foreach (var sizeid in product.SizeIds)
                {
                    if (_context.Sizes.Any(x => x.Id == sizeid))
                    {
                        ProductSize size = new ProductSize { SizeId = sizeid };
                        product.ProductSizes.Add(size);
                    }
                    else
                    {
                        ModelState.AddModelError("Sizeids", "Size not found");
                        return View();
                    }


                }
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Product product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).Include(x => x.ProductSizes).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.Include(x => x.ProductColors).ThenInclude(x => x.Product).ToList();
            ViewBag.Sizes = _context.Sizes.Include(x => x.ProductSizes).ThenInclude(x => x.Product).ToList();

            product.ColorIds = product.ProductColors.Select(x => x.ColorId).ToList();
            product.SizeIds = product.ProductSizes.Select(x => x.SizeId).ToList();
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {

            Product existproduct = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x => x.Color).Include(x => x.ProductSizes).ThenInclude(x => x.Size).FirstOrDefault(x => x.Id == product.Id);

            if (existproduct == null) return NotFound();

            if (!_context.Brands.Any(x => x.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand not found");
                return View();
            }
            if (!_context.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View();
            }


            if (product.PosterFile != null && product.PosterFile.ContentType != "image/jpeg" && product.PosterFile.ContentType != "image/png")
            {
                ModelState.AddModelError("PosterFile", "file type must be image/jpeg or image/png");
                return View();
            }

            if (product.PosterFile != null && product.PosterFile.Length > 2097152)
            {
                ModelState.AddModelError("PosterFile", "file size must be less than 2mb");
                return View();
            }

            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {
                    if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Files", "file type must be image/jpeg or image/png");
                        return View();
                    }

                    if (file.Length > 2097152)
                    {
                        ModelState.AddModelError("Files", "file size must be less than 2mb");
                        return View();
                    }
                }
            }

            ProductImage poster = existproduct.ProductImages.FirstOrDefault(x => x.PosterStatus == true);



            if (product.PosterFile != null)
            {
                string newPosterImg = FileManager.Save(_env.WebRootPath, "uploads/products", product.PosterFile);
                if (poster != null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/products", poster.Image);
                    poster.Image = newPosterImg;
                }
                else
                {
                    poster = new ProductImage { Image = newPosterImg, PosterStatus = true };
                    existproduct.ProductImages.Add(poster);
                }
            }
            existproduct.ProductColors = new List<ProductColor>();
            existproduct.ProductSizes = new List<ProductSize>();

            if (product.ColorIds != null)
            {

                foreach (var colorid in product.ColorIds)
                {

                    ProductColor productcolor = new ProductColor
                    {
                        ColorId = colorid,
                    };
                    existproduct.ProductColors.Add(productcolor);
                }
            }
            if (product.SizeIds != null)
            {

                foreach (var sizeid in product.SizeIds)
                {

                    ProductSize productSize = new ProductSize
                    {
                        SizeId = sizeid,
                    };
                    existproduct.ProductSizes.Add(productSize);
                }
            }

            existproduct.ProductImages.RemoveAll(x => x.PosterStatus == null && !product.FileIds.Contains(x.Id));

            if (product.Files != null)
            {
                foreach (var file in product.Files)
                {
                    ProductImage productimage = new ProductImage
                    {
                        PosterStatus = null,
                        Image = FileManager.Save(_env.WebRootPath, "uploads/products", file)
                    };
                    existproduct.ProductImages.Add(productimage);
                }
            }


            existproduct.CategoryId = product.CategoryId;
            existproduct.BrandId = product.BrandId;
            existproduct.CostPrice = product.CostPrice;
            existproduct.SalePrice = product.SalePrice;
            existproduct.IsAvailable = product.IsAvailable;
            existproduct.IsMan = product.IsMan;
            existproduct.IsNew = product.IsNew;
            existproduct.IsSpecial = product.IsSpecial;
            existproduct.IsDeleted = product.IsDeleted;
            existproduct.IsFeatured = product.IsFeatured;
            existproduct.Name = product.Name;
            existproduct.Desc = product.Desc;
            existproduct.DiscountPercent = product.DiscountPercent;


            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).ThenInclude(x => x.Color).Include(x => x.ProductSizes).ThenInclude(x => x.Size).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();



            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {


            Product existproduct = _context.Products.FirstOrDefault(x => x.Id == product.Id);

            if (existproduct == null) return NotFound();

            //if (existproduct.BackImage != null)
            //{
            //    string path = Path.Combine(_env.WebRootPath, "uploads/slider", existproduct.BackImage);
            //    if (System.IO.File.Exists(path))
            //    {
            //        System.IO.File.Delete(path);
            //    }
            //}


            _context.Products.Remove(existproduct);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
