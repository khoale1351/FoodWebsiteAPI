    namespace FoodWebsite_API.DTOs.Recipe
{
    public class RecipeSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? NamePlain { get; set; }
        public bool IsOriginal { get; set; }
        public int? PrepareTime { get; set; }
        public int CookingTime { get; set; }
        public string? Description { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string SpecialtyName { get; set; } = null!;
        public int TotalSteps { get; set; }
        public int TotalIngredients { get; set; }
    }
}
