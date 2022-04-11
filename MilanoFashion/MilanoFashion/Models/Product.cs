using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        public string Name { get; set; }
        [StringLength(maximumLength: 500)]
        public string Desc { get; set; }
        public bool IsMan { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int CategoryId { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public Category Category { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsNew { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsDeleted { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        [NotMapped]
        public List<int> ColorIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> SizeIds { get; set; } = new List<int>();
        [NotMapped]
        public IFormFile PosterFile { get; set; }
        [NotMapped]
        public List<IFormFile> Files { get; set; }
        [NotMapped]
        public List<int> FileIds { get; set; } = new List<int>();
        public List<ProductComment> ProductComments { get; set; }
        public List<OrderItem> OrderItems { get; set; }



    }
}
