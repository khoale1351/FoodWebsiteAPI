using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.RecipeIngredient
{
    public class RecipeIngredientUpdateDTO
    {
        [Required]
        public int IngredientId { get; set; }

        [Required]
        [Range(0.01, 10000, ErrorMessage = "Định lượng phải nằm trong khoảng 0.01 đến 10000")]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = null!;
    }
}
