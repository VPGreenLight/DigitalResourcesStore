﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.OrderHistoryDtos
{
    public class OrderHistoryDetailDto
    {
        public int ProductDetailId { get; set; }
        public int OrderHistoryId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string Serial { get; set; }
        public string Code { get; set; }
    }
}
