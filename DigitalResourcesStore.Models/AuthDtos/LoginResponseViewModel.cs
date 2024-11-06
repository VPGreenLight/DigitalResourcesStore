﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.AuthDtos
{
    public class LoginResponseViewModel
    {
        public string UserInformation { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}