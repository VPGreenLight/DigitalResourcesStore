﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.DepositDtos
{
    public class UserBalanceDto
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
