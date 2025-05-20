namespace FoodWebsite_API.Models
{
    public class RecipeStep
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int StepNumber { get; set; }

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;
    }
}
