using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MilanoFashion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Dal
{
    public class DataContext:IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }

        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FoundersInfoSlider> FoundersInfoSliders { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BrandLogo> BrandLogos { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Faqs> Faqss { get; set; }

    }
}
