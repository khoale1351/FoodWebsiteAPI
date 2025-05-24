namespace FoodWebsite_API.DTOs
{
    public class BaseFilterDTO
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
