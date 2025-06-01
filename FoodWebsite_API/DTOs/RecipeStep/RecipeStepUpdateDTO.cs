using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.RecipeStep
{
    public class RecipeStepUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 999)]
        public int StepNumber { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        public IFormFile Image { get; set; }
    }
}
