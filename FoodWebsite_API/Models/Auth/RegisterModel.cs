using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class RegisterModel
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
