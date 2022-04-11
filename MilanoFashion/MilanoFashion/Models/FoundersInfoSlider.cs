using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class FoundersInfoSlider
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 150)]
        public string Name { get; set; }
        [StringLength(maximumLength: 200)]

        public string Image { get; set; }
        [StringLength(maximumLength: 500)]

        public string Bio { get; set; }
        [StringLength(maximumLength: 150)]

        public string Position { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }


    }
}
