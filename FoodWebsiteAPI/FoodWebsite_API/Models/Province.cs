using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class Province
    {
        public int Id { get; set; }

        public string Region { get; set; } = null!;

        public string? RegionPlain { get; set; }

        public string Name { get; set; } = null!;

        public string? NamePlain { get; set; }

        public string? Description { get; set; }

        public int Version { get; set; }

        public bool IsActive { get; set; }

        public string? ImageUrl { get; set; }

        public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
    }
}
