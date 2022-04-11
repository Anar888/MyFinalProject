﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class AppUser: IdentityUser
    {
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public List<ProductComment> ProductComments { get; set; }

    }
}
