using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.Models
{
    public class NguyenLieu
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Ten { get; set; }

        [StringLength(100)]
        public string TenKhongDau { get; set; }

        [StringLength(100)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<NguyenLieuCongThuc> NguyenLieuCongThucs { get; set; }

        public ICollection<NguoiDungNguyenLieu> NguoiDungNguyenLieus { get; set; }

        public NguyenLieu() => CreatedAt = DateTime.UtcNow;
    }
}
