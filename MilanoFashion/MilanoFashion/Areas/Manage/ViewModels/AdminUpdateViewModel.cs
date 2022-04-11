using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Areas.Manage.ViewModels
{
    public class AdminUpdateViewModel
    {
        [Required]
        [StringLength(maximumLength: 30)]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string Email { get; set; }
        [StringLength(maximumLength: 20)]
        public string Phone { get; set; }
        [StringLength(maximumLength: 50)]
        public string Country { get; set; }
        [StringLength(maximumLength: 50)]
        public string City { get; set; }
        [StringLength(maximumLength: 400)]
        public string Address { get; set; }
        [StringLength(maximumLength: 25)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [StringLength(maximumLength: 25)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [StringLength(maximumLength: 25)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
