using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ hoặc sai định dạng")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ thường và một chữ hoa")]
        public string Password { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Số điện thoại phải từ 9 đến 15 ký tự")]
        public string? PhoneNumber { get; set; }
    }
}
