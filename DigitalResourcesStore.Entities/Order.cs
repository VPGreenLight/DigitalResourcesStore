using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("Order")]
public partial class Order
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    public int? Quantity { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [Column("VoucherID")]
    public int? VoucherId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Orders")]
    public virtual Product? Product { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }

    [ForeignKey("VoucherId")]
    [InverseProperty("Orders")]
    public virtual Voucher? Voucher { get; set; }
}
