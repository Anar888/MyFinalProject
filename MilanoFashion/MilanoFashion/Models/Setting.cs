using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string Key { get; set; }
        [Required]
        [StringLength(maximumLength: 500)]
        public string Value { get; set; }
    }
}
