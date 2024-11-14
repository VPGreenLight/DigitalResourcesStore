using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.CartsDtos
{
    public class CartSummaryDto
    {
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }
}
