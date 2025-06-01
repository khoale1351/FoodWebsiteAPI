using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.RecipeStep
{
    public class RecipeStepCreateDTO
    {
        [Required]
        [Range(1, 999)]
        public int StepNumber { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
