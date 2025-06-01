namespace FoodWebsite_API.DTOs.RecipeStep
{
    public class RecipeStepReadDTO
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
