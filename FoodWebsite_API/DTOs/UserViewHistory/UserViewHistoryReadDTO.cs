namespace FoodWebsite_API.DTOs.UserViewHistory
{
    public class UserViewHistoryReadDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int? SpecialtyId { get; set; }
        public int? RecipeId { get; set; }
        public DateTime ViewedAt { get; set; }
        public string? UserName { get; set; }
        public string? RecipeName { get; set; }
        public string? SpecialtyName { get; set; }
    }
}
