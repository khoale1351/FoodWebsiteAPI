namespace FoodWebsite_API.DTOs.Recipe
{
    public class RecipeSearchDTO : BaseFilterDTO
    {
        public string? SearchTerm { get; set; }
        public int? SpecialtyId { get; set; }
        public bool? IsOriginal { get; set; }
        public bool? IsApproved { get; set; }
        public int? MaxPrepareTime { get; set; }
        public int? MaxCookingTime { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }
}
