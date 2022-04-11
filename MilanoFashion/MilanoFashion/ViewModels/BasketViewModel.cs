using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.ViewModels
{
    public class BasketViewModel
    {
        public List<ProductBasketItemViewModel> Products { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
