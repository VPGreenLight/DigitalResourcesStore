using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("MessageSupport")]
public partial class MessageSupport
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    public string Subject { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsProcessed { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    public int? AdminReplyId { get; set; }

    [StringLength(100)]
    public string? UserName { get; set; }

    [ForeignKey("AdminReplyId")]
    [InverseProperty("MessageSupports")]
    public virtual AdminReply? AdminReply { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("MessageSupports")]
    public virtual User? User { get; set; }
}
