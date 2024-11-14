using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DepositDtos
{
    public class DepositDto
    {
        [Range(10000, 500000, ErrorMessage = "Số tiền chỉ trong khoảng từ 10,000 đến 500,000")]
        public int amount { get; set; }
        public string? description { get; set; }
    }
}
