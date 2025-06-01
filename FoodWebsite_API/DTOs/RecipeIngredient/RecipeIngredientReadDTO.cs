namespace FoodWebsite_API.DTOs.RecipeIngredient
{
    public class RecipeIngredientReadDTO
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
    }
}
