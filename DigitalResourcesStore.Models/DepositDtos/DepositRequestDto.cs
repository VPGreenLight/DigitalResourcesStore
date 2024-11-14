using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DepositDtos
{
    public class DepositRequestDto
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
    }
}
