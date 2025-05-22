using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWebsite_API.Models
{
    public class Specialty
    {
        public int Id { get; set; }

        public int ProvinceId { get; set; }

        public string Name { get; set; } = null!;

        public string? NamePlain { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual Province Province { get; set; } = null!;

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

        public virtual ICollection<SpecialtyImage> SpecialtyImages { get; set; } = new List<SpecialtyImage>();

        public virtual ICollection<UserViewHistory> UserViewHistories { get; set; } = new List<UserViewHistory>();
    }
}
