using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class Banner
    {
        public int Id { get; set; }
     
        public string ImageUrl { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
