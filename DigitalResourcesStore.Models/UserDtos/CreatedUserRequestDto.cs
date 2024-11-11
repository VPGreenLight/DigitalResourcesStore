using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.UserDtos
{
    public class CreatedUserRequestDto
    {
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [MinLength(6, ErrorMessage = "{0} phải có ít nhất {1} ký tự")]
        [Display(Name = "mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} không được để trống")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp với mật khẩu")]
        [Display(Name = "xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "số điện thoại")]
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public int RoleId { get; set; }
    }
}
