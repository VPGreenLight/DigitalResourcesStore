using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.UserFavoriteDtos
{
    public class UserFavoriteResponse
    {
        public int UserId { get; set; }
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
    }
}
