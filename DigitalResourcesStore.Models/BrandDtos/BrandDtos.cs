using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.BrandDtos
{
    public class BrandDtos
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên thương hiệu không được vượt quá 255 ký tự.")]
        public string Name { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [StringLength(255, ErrorMessage = "Người tạo không được vượt quá 255 ký tự.")]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [StringLength(255, ErrorMessage = "Người cập nhật không được vượt quá 255 ký tự.")]
        public string? UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DeletedAt { get; set; }

        [Column("isDelete")]
        public bool? IsDelete { get; set; }

        [StringLength(255, ErrorMessage = "Người xóa không được vượt quá 255 ký tự.")]
        public string? DeletedBy { get; set; }
    }
}
