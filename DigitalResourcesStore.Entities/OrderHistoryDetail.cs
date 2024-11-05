using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("OrderHistoryDetail")]
public partial class OrderHistoryDetail
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ProductDetailID")]
    public int ProductDetailId { get; set; }

    [Column("OrderHistoryID")]
    public int OrderHistoryId { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    [StringLength(255)]
    public string? Serial { get; set; }

    [StringLength(255)]
    public string? Code { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("OrderHistoryDetails")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("OrderHistoryId")]
    [InverseProperty("OrderHistoryDetails")]
    public virtual OrderHistory OrderHistory { get; set; } = null!;

    [ForeignKey("ProductDetailId")]
    [InverseProperty("OrderHistoryDetails")]
    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
