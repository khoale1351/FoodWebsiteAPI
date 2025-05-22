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
        public string FullName { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string City { get; set; } = string.Empty;

        public GenderType Gender { get; set; }

        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public MembershipType Membership { get; set; }

        public DateTime CreatedAt { get; set; }

        //Navigation Properties
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        public ICollection<UserIngredient> UserIngredients { get; set; } = new List<UserIngredient>();

        public ICollection<UserFavoriteRecipe> UserFavoriteRecipes { get; set; } = new List<UserFavoriteRecipe>();

        public ICollection<UserViewHistory> UserViewHistories { get; set; } = new List<UserViewHistory>();

        public ApplicationUser() => CreatedAt = DateTime.UtcNow;
    }
}
