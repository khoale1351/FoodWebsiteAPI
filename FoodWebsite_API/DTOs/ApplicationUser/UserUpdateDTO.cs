using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.ApplicationUser
{
    public class UserUpdateDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? City { get; set; }

        public string? Address { get; set; }

        [EnumDataType(typeof(GenderType))]
        public GenderType Gender { get; set; }

        [EnumDataType(typeof(MembershipType))]
        public MembershipType Membership { get; set; }
    }

    public enum GenderType
    {
        Nam = 0,
        Nu = 1,
        Khac = 2
    }

    public enum MembershipType
    {
        Normal,
        Premium,
        VIP
    }
}
