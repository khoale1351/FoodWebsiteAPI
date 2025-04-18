using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWebsite_API.Models
{
    public class DacSan
    {
        public int Id { get; set; }

        [Required]
        public int TinhThanhId { get; set; }

        [Required]
        [StringLength(100)]
        public string Ten { get; set; }

        [StringLength(100)]
        public string TenKhongDau { get; set; }

        [StringLength(100)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public TinhThanh TinhThanh { get; set; }

        public ICollection<DacSanImages> DacSanImages { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<CongThuc> CongThucs { get; set; }

        public DacSan() => CreatedAt = DateTime.UtcNow;

        [NotMapped]
        public double DiemDanhGiaTrungBinh { get; set; }

        [NotMapped]
        public int SoLuongDanhGia { get; set; }
    }
}
