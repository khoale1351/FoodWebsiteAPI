using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class Ingredient
    {
        public int Id { get; set; }



        public string Name { get; set; } = null!;

        public string? NamePlain { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        public virtual ICollection<UserIngredient> UserIngredients { get; set; } = new List<UserIngredient>();
    }
}
