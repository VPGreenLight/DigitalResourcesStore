using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.ProductDetailDtos
{
    public class ProductDetailDtos
    {
        //[Required(ErrorMessage = "Vui lòng nhập mã định danh.")]
        [Display(Name = "Mã định danh")]
        public int Id { get; set; }

        [Display(Name = "Số serial")]
        public string? Serial { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public string? Code { get; set; }

        [Display(Name = "ID sản phẩm liên kết")]
        public int? ProductId { get; set; }

        [Display(Name = "Đã xóa")]
        public bool IsDelete { get; set; }
    }
}
