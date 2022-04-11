using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Position { get; set; }
        public string Image { get; set; }
        public string Fburl { get; set; }
        public string Twiturl { get; set; }
        public string Youtubeurl { get; set; }
        public string Googleurl { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
