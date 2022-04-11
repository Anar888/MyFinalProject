using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; }
        public List<ProductColor> ProductColors { get; set; }
    }
}
