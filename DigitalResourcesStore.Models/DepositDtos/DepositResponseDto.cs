using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DepositDtos
{
    public class DepositResponseDto
    {
        public int TransactionId { get; set; }
        public string Status { get; set; }
        public decimal NewBalance { get; set; }
        public string Message { get; set; }
    }
}
