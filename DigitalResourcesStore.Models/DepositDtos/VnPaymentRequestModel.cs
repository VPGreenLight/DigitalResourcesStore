using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DepositDtos
{
    public class VnPaymentRequestModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public float Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
