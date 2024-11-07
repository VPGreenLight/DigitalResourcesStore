using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalResourcesStore.Models.ProductDtos
{
    public class CreatedProductDtos
    {
        //public int Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được quá 100 ký tự.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Hình ảnh")]
        public string? Image { get; set; }

        [Display(Name = "Ngày hết hạn")]
        public DateTime? Expiry { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự.")]
        public string? Description { get; set; }

        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Số lượng")]
        public int? Quantity { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "Thương hiệu")]
        public int? BrandId { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Người tạo")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Người cập nhật")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "Ngày xóa")]
        public DateTime? DeletedAt { get; set; }

        [Display(Name = "Đã xóa")]
        public bool? IsDelete { get; set; }

        [Display(Name = "Người xóa")]
        public string? DeletedBy { get; set; }
    }
}
