using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DashBoardDtos
{
    public class BestSellingProductDtos
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? TotalQuantitySold { get; set; }
    }
}
