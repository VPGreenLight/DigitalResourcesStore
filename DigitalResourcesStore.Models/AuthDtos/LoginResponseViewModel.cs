using DigitalResourcesStore.Models.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.AuthDtos
{
    public class LoginResponseViewModel
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
