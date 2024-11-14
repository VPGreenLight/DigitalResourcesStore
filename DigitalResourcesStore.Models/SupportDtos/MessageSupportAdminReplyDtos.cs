using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.SupportDtos
{
    public class MessageSupportAdminReplyDtos
    {
        public int MessageSupportId { get; set; }
        public string ReplyContent { get; set; }
        public DateTime ReplyDate { get; set; }
    }
}
