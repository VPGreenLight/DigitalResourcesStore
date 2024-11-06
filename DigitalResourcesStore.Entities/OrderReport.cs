using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("OrderReport")]
public partial class OrderReport
{
    [Key]
    public int Id { get; set; }

    public int? OrderHistoryId { get; set; }

    [StringLength(255)]
    public string ReportedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ReportDate { get; set; }

    [StringLength(255)]
    public string Issue { get; set; } = null!;

    public bool? IsResolved { get; set; }

    [ForeignKey("OrderHistoryId")]
    [InverseProperty("OrderReports")]
    public virtual OrderHistory? OrderHistory { get; set; }
}
