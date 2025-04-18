using FoodWebsite_API.Function;
using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class CongThuc
    {
        public int Id { get; set; }

        [Required]
        public int DacSanId { get; set; }

        [Required]
        [StringLength(200)]
        public string Ten { get; set; }

        [StringLength(200)]
        public string TenKhongDau { get; set; }

        [StringLength(200)]
        public string Slug { get; set; }

        public bool IsChinhGoc { get; set; } = false;

        [Required]
        public string HuongDan { get; set; }

        [Range(0, 600)]
        public int ThoiGianChuanBi { get; set; }

        [Range(0, 600)]
        public int ThoiGianNau { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public DacSan DacSan { get; set; }

        public ICollection<NguyenLieuCongThuc> NguyenLieu_CongThucs { get; set; }

        public CongThuc() => CreatedAt = DateTime.UtcNow;
    }
}
