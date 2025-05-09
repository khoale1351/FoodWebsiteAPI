using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class SpecialtyImage
    {
        public int Id { get; set; }

        public int SpecialtyId { get; set; }

        public string? ImageUrl { get; set; }

        public virtual Specialty Specialty { get; set; } = null!;
    }
}
