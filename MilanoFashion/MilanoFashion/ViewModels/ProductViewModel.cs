using MilanoFashion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.ViewModels
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Color> Colors { get; set; }
        public List<Category> Categories { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
    }
}
