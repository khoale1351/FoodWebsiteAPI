using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int SpecialtyId { get; set; }

        public int Stars { get; set; }

        public string? Comment { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Specialty Specialty { get; set; } = null!;

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
