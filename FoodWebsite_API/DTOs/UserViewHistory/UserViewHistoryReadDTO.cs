namespace FoodWebsite_API.DTOs.UserViewHistory
{
    public class UserViewHistoryReadDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? SpecialtyName { get; set; }
        public string? RecipeName { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
