using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class RegisterModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
