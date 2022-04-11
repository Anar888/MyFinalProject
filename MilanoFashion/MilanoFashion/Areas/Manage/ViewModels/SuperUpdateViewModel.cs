using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Areas.Manage.ViewModels
{
    public class SuperUpdateViewModel
    {
        [Required]
        [StringLength(maximumLength: 30)]
        public string FullName { get; set; }
        [StringLength(maximumLength: 30)]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength: 30)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(maximumLength: 30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
