using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("ContactRequest")]
public partial class ContactRequest
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [StringLength(255)]
    public string FullName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    [Column("CCCD")]
    [StringLength(20)]
    public string Cccd { get; set; } = null!;

    [StringLength(255)]
    public string? FacebookLink { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    public string? Content { get; set; }

    public bool? IsAccepted { get; set; }

    public int? AdminReplyId { get; set; }

    [ForeignKey("AdminReplyId")]
    [InverseProperty("ContactRequests")]
    public virtual AdminReply? AdminReply { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ContactRequests")]
    public virtual User? User { get; set; }
}
