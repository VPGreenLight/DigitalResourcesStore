using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DigitalResourcesStore.Models.SupportDtos
{
    public class ContactRequestDtos
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Tên đầy đủ không thể quá 255 ký tự.")]
        public string FullName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Cccd phải có 12 số.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Cccd phải là số và phải có 12 số.")]
        public string Cccd { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Liên kết Facebook không thể quá 255 ký tự.")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?facebook\.com(\/[A-Za-z0-9.]{1,})?$", ErrorMessage = "Liên kết Facebook phải là URL hợp lệ của Facebook.")]
        public string FacebookLink { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Số điện thoại không thể quá 10 chữ số.")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0 hoặc +84.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Email không thể quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Nội dung không thể quá 1000 ký tự.")]
        public string Content { get; set; }
        public bool? IsAccepted {  get; set; }
        public bool? AdminReply {  get; set; }
        //public int? UserId { get; set; }
    }
}
