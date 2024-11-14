using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.SupportDtos
{
    public class MessageSupportDtos
    {
        [Required]
        [StringLength(255, ErrorMessage = "Email không thể quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [StringLength(10, ErrorMessage = "Số điện thoại không thể quá 10 chữ số.")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0 hoặc +84.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Tiêu đề không thể quá 255 ký tự.")]
        public string Subject { get; set; } = null!;

        [Required]
        [StringLength(1000, ErrorMessage = "Nội dung không thể quá 1000 ký tự.")]
        public string Content { get; set; } = null!;
        public bool IsProcessed { get; set; }
        public int? UserId { get; set; }
    }
}
