using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.UserDtos
{
    public class UserLoginDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RoleName { get; set; }
        public decimal? Money { get; set; }
        public string VipRank { get; set; }
    }
}
