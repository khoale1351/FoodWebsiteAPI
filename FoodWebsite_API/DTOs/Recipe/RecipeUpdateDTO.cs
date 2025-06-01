using FoodWebsite_API.DTOs.RecipeIngredient;
using FoodWebsite_API.DTOs.RecipeStep;
using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.Recipe
{
    public class RecipeUpdateDTO
    {
        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        public string? NamePlain { get; set; }

        public bool IsOriginal { get; set; }

        [Range(0, 6000)]
        public int? PrepareTime { get; set; }

        [Required]
        [Range(0, 6000)]
        public int CookingTime { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsApproved { get; set; }

        public List<RecipeStepUpdateDTO> RecipeSteps { get; set; } = new List<RecipeStepUpdateDTO>();
        public List<RecipeIngredientUpdateDTO> RecipeIngredients { get; set; } = new List<RecipeIngredientUpdateDTO>();
    }
}
