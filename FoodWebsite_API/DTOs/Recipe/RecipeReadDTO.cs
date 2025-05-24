namespace FoodWebsite_API.DTOs.Recipe
{
    public class RecipeReadDTO
    {
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = null!;
        public string? NamePlain { get; set; }
        public bool IsOriginal { get; set; }
        public int? PrepareTime { get; set; }
        public int CookingTime { get; set; }
        public string? Description { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
    }
}
