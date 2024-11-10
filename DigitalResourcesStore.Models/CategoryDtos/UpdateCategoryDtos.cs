using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.CategoryDtos
{
    public class UpdateCategoryDtos
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên không được vượt quá 255 ký tự.")]
        public string? Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }

        [StringLength(255, ErrorMessage = "Người tạo không được vượt quá 255 ký tự.")]
        public string? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [StringLength(255, ErrorMessage = "Người cập nhật không được vượt quá 255 ký tự.")]
        public string? UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }

        public bool? IsDelete { get; set; }

        [StringLength(255, ErrorMessage = "Người xóa không được vượt quá 255 ký tự.")]
        public string? DeletedBy { get; set; }
    }
}
