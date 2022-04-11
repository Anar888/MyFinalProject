using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}
