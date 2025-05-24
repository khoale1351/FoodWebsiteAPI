namespace FoodWebsite_API.DTOs.SpecialtyImages
{
    public class SpecialtyImageMultiUploadDTO
    {
        public int SpecialtyId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
