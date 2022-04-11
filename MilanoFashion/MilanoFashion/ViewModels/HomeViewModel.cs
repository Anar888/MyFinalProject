using MilanoFashion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> News { get; set; }
        public List<Product> Featured { get; set; }
        public List<Product> Special { get; set; }
        public List<FoundersInfoSlider> FoundersInfoSliders { get; set; }
        public List<Shipping> Shippings { get; set; }
        public List<Banner> Banners { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<BrandLogo> BrandLogos { get; set; }
    }
}
