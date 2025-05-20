using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.Province
{
    public class ProvinceCreateDTO
    {
        [Required]
        public string Region { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int Version { get; set; }
        public string ImageUrl { get; set; }
    }
}
