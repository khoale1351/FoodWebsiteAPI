namespace FoodWebsite_API.DTOs.Specialty
{
    public class SpecialtyListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NamePlain { get; set; }
        public string Description { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
