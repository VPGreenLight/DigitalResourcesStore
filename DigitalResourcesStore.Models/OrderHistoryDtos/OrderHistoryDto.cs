using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.OrderHistoryDtos
{
    public class OrderHistoryDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime? Date { get; set; }
        public int? Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderHistoryDetailDto> OrderDetails { get; set; }
    }
}
