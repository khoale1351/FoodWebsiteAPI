namespace FoodWebsite_API.DTOs.SpecialtyImages
{
    public class SpecialtyImagesReadDTO
    {
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
