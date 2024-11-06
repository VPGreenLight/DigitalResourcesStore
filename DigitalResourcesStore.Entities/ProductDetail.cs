using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("ProductDetail")]
public partial class ProductDetail
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    public string? Serial { get; set; }

    [StringLength(255)]
    public string? Code { get; set; }

    public int? ProductId { get; set; }

    [Column("isDelete")]
    public bool IsDelete { get; set; }

    [InverseProperty("ProductDetail")]
    public virtual ICollection<OrderHistoryDetail> OrderHistoryDetails { get; set; } = new List<OrderHistoryDetail>();

    [ForeignKey("ProductId")]
    [InverseProperty("ProductDetails")]
    public virtual Product? Product { get; set; }
}
