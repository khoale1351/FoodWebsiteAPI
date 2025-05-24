using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.Province
{
    public class ProvinceUpdateDTO
    {
        [Required]
        public string Region { get; set; }
        public string RegionPlain { get; set; }
        [Required]
        public string Name { get; set; }
        public string NamePlain { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }
}
