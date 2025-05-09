using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWebsite_API.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }

        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; } = null!;

        public virtual Ingredient Ingredient { get; set; } = null!;

        public virtual Recipe Recipe { get; set; } = null!;
    }
}
