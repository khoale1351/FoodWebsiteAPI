using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWebsite_API.Models
{
    public class NguyenLieuCongThuc
    {
        [Key, Column(Order = 0)]
        public int CongThucId { get; set; }

        [Key, Column(Order = 1)]
        public int NguyenLieuId { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal SoLuong { get; set; }

        [Required]
        [StringLength(50)]
        public string DonVi { get; set; }

        [ForeignKey("CongThucId")]
        public CongThuc CongThuc { get; set; }

        [ForeignKey("NguyenLieuId")]
        public NguyenLieu NguyenLieu { get; set; }
    }
}
