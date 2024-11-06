using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("Menu")]
public partial class Menu
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Position { get; set; }

    [StringLength(255)]
    public string? Links { get; set; }

    [StringLength(255)]
    public string? Location { get; set; }

    [Column("URL")]
    [StringLength(255)]
    public string? Url { get; set; }
}
