using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("AdminReply")]
public partial class AdminReply
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public string ReplyContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ReplyDate { get; set; }

    [InverseProperty("AdminReply")]
    public virtual ICollection<ContactRequest> ContactRequests { get; set; } = new List<ContactRequest>();

    [InverseProperty("AdminReply")]
    public virtual ICollection<MessageSupport> MessageSupports { get; set; } = new List<MessageSupport>();
}
