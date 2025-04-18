using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class DacSanImages
    {
        public int Id { get; set; }

        [Required]
        public int DacSanId { get; set; }

        public string ImageUrl { get; set; }

        public DacSan DacSan { get; set; }
    }
}
