namespace FoodWebsite_API.Models
{
    public class UserFavoriteRecipe
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int RecipeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
