using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("VipCustomer")]
public partial class VipCustomer
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal RequiredAmount { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal CashbackRate { get; set; }

    public string? Benefits { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [Column("isDelete")]
    public bool? IsDelete { get; set; }

    public int? RankId { get; set; }

    [ForeignKey("RankId")]
    [InverseProperty("VipCustomers")]
    public virtual TypeRank? Rank { get; set; }

    [InverseProperty("VipCustomer")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
