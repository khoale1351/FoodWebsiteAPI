using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class UserViewHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public int? SpecialtyId { get; set; }

        public int? RecipeId { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public virtual Specialty? Specialty { get; set; }

        public virtual Recipe? Recipe { get; set; }
    }
}
