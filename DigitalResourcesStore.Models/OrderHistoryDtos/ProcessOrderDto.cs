using DigitalResourcesStore.Models.CartsDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.OrderHistoryDtos
{
    public class ProcessOrderDto
    {
        public int UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
        public decimal DiscountedTotal { get; set; }
    }
}
