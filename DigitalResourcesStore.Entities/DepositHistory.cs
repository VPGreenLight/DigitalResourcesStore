using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("DepositHistory")]
public partial class DepositHistory
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedBy { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Money { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("isSuccess")]
    public bool? IsSuccess { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("DepositHistories")]
    public virtual User? User { get; set; }
}
