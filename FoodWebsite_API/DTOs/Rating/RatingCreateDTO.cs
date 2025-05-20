namespace FoodWebsite_API.DTOs.Rating
{
    public class RatingCreateDTO
    {
        public int SpecialtyId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }
}
