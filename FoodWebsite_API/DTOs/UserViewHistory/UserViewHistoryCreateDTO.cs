using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.UserViewHistory
{
    public class UserViewHistoryCreateDTO
    {
        [Required]
        public string UserId { get; set; } = null!;
        public int? SpecialtyId { get; set; }
        public int? RecipeId { get; set; }
    }
}
