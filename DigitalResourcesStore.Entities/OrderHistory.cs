using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("OrderHistory")]
public partial class OrderHistory
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    [InverseProperty("OrderHistory")]
    public virtual ICollection<OrderHistoryDetail> OrderHistoryDetails { get; set; } = new List<OrderHistoryDetail>();

    [InverseProperty("OrderHistory")]
    public virtual ICollection<OrderReport> OrderReports { get; set; } = new List<OrderReport>();

    [ForeignKey("ProductId")]
    [InverseProperty("OrderHistories")]
    public virtual Product? Product { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("OrderHistories")]
    public virtual User? User { get; set; }
}
