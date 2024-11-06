using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("User")]
public partial class User
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(255)]
    public string UserName { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("RoleID")]
    public int? RoleId { get; set; }

    [Column("isActive")]
    public bool? IsActive { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Money { get; set; }

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

    [StringLength(255)]
    public string? VipRank { get; set; }

    [StringLength(50)]
    public string? ReferralCode { get; set; }

    public int MembershipPoints { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FriendReferralCode { get; set; }

    [Column("isInvitedSuccess")]
    public bool IsInvitedSuccess { get; set; }

    public int? VipCustomerId { get; set; }

    public int? TypeRankId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ContactRequest> ContactRequests { get; set; } = new List<ContactRequest>();

    [InverseProperty("User")]
    public virtual ICollection<DepositHistory> DepositHistories { get; set; } = new List<DepositHistory>();

    [InverseProperty("User")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("User")]
    public virtual ICollection<MessageSupport> MessageSupports { get; set; } = new List<MessageSupport>();

    [InverseProperty("User")]
    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Referred")]
    public virtual ICollection<ReferralProgram> ReferralProgramReferreds { get; set; } = new List<ReferralProgram>();

    [InverseProperty("Referrer")]
    public virtual ICollection<ReferralProgram> ReferralProgramReferrers { get; set; } = new List<ReferralProgram>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }

    [ForeignKey("TypeRankId")]
    [InverseProperty("Users")]
    public virtual TypeRank? TypeRank { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();

    [ForeignKey("VipCustomerId")]
    [InverseProperty("Users")]
    public virtual VipCustomer? VipCustomer { get; set; }
}
