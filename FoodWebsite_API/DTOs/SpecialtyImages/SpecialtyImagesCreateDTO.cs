namespace FoodWebsite_API.DTOs.SpecialtyImages
{
    public class SpecialtyImagesCreateDTO
    {
        public int SpecialtyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
