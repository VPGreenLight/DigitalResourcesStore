using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.SupportDtos
{
    public class AdminReplyDtos
    {
        public string ReplyContent { get; set; }
        public DateTime ReplyDate { get; set; }
        public int? ContactRequestId { get; set; } // Sẽ có thể là null
        public int? MessageSupportId { get; set; } // Sẽ có thể là null
    }
}
