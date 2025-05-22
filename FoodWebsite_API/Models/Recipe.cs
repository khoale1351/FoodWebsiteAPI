using FoodWebsite_API.Function;
using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class Recipe
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

        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        public virtual ICollection<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();

        public virtual Specialty Specialty { get; set; } = null!;

        public virtual ICollection<UserFavoriteRecipe> UserFavoriteRecipes { get; set; } = new List<UserFavoriteRecipe>();

        public virtual ICollection<UserViewHistory> UserViewHistories { get; set; } = new List<UserViewHistory>();
    }
}
