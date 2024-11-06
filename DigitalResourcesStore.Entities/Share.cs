using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework.Models;

[Table("Share")]
public partial class Share
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("author")]
    [StringLength(255)]
    public string? Author { get; set; }

    [Column("image")]
    [StringLength(255)]
    public string? Image { get; set; }

    [Column("date", TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("detail")]
    public string? Detail { get; set; }

    [Column("isdelete")]
    public bool? Isdelete { get; set; }
}
