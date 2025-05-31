namespace FoodWebsite_API.DTOs.ApplicationUser
{
    public class UserReadDTO
    {
        public string Id { get; set; } = null!;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Gender { get; set; } = "Khac";

        public string Membership { get; set; } = "Normal";

        public DateTime CreatedAt { get; set; }
    }
}
