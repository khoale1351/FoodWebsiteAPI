using FoodWebsite_API.DTOs.RecipeIngredient;
using FoodWebsite_API.DTOs.RecipeStep;
using FoodWebsite_API.Models;

namespace FoodWebsite_API.DTOs.Recipe
{
    public class RecipeDetailDTO
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
        public string SpecialtyName { get; set; } = null!;
        public List<RecipeStepReadDTO> RecipeSteps { get; set; } = new List<RecipeStepReadDTO>();
        public List<RecipeIngredientReadDTO> RecipeIngredients { get; set; } = new List<RecipeIngredientReadDTO>();
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
    }
}
