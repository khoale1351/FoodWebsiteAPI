using Microsoft.AspNetCore.Identity;

namespace FoodWebsite_API.Models
{
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

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string AvatarUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string City { get; set; }

        public GenderType Gender { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public MembershipType Membership { get; set; }

        public DateTime CreatedAt { get; set; }

        //Navigation Properties
        public ICollection<Review> Reviews { get; set; }

        public ICollection<NguoiDungNguyenLieu> NguoiDung_NguyenLieus { get; set; }

        public ApplicationUser() => CreatedAt = DateTime.UtcNow;
    }
}
