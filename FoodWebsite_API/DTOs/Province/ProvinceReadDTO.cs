namespace FoodWebsite_API.DTOs.Province
{
    public class ProvinceReadDTO
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string RegionPlain { get; set; }
        public string Name { get; set; }
        public string NamePlain { get; set; }
        public string? Description { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }
}
