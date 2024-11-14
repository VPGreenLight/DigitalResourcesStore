using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.CartsDtos
{
    public class UpdateCartQuantityDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
