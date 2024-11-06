using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("ReferralProgram")]
public partial class ReferralProgram
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ReferrerID")]
    public int ReferrerId { get; set; }

    [Column("ReferredID")]
    public int ReferredId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReferralDate { get; set; }

    public string? Conditions { get; set; }

    [StringLength(255)]
    public string? Reward { get; set; }

    public bool? IsSuccessful { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CompletionDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiryDate { get; set; }

    [ForeignKey("ReferredId")]
    [InverseProperty("ReferralProgramReferreds")]
    public virtual User Referred { get; set; } = null!;

    [ForeignKey("ReferrerId")]
    [InverseProperty("ReferralProgramReferrers")]
    public virtual User Referrer { get; set; } = null!;
}
