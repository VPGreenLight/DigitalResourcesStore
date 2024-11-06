using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public string? Description { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column("ReplyID")]
    public int? ReplyId { get; set; }

    public bool? IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [StringLength(255)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(255)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [Column("isDelete")]
    public bool? IsDelete { get; set; }

    [StringLength(255)]
    public string? DeletedBy { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Feedbacks")]
    public virtual Product? Product { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Feedbacks")]
    public virtual User? User { get; set; }
}
