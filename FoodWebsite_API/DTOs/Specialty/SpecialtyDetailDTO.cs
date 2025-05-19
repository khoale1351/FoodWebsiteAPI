namespace FoodWebsite_API.DTOs.Specialty
{
    public class SpecialtyDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? NamePlain { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ProvinceName { get; set; } = null!;
        public List<string> ImageUrls { get; set; } = new();
    }
}
