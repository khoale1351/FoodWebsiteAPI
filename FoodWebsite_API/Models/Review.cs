using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int DacSanId { get; set; }

        [Required]
        [Range(1,5)]
        public int SoSao { get; set; }

        [StringLength(1000)]
        public string BinhLuan { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }

        public DacSan DacSan { get; set; }

        public Review() => CreatedAt = DateTime.UtcNow;
    }
}
