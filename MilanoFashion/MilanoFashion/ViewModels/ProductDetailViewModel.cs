using MilanoFashion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public ProductComment Comment { get; set; }
        public ProductImage Images { get; set; }

    }
}
