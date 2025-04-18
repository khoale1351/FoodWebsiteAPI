using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class TinhThanh
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string VungMien { get; set; }

        [StringLength(50)]
        public string VungMienKhongDau { get; set; }

        [Required]
        [StringLength(100)]
        public string Ten { get; set; }

        [StringLength(100)]
        public string TenKhongDau { get; set; }

        [StringLength(100)]
        public string Slug { get; set; }

        public string MoTa { get; set; }

        public string HinhAnh { get; set; }

        public int Version { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<DacSan> DacSans { get; set; }
    }
}
