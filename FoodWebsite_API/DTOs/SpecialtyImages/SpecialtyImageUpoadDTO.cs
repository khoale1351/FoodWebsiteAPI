using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.SpecialtyImages
{
    public class SpecialtyImageUpoadDTO
    {
        [Required]
        public int SpecialtyId { get; set; }
        [Required]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
