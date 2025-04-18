using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodWebsite_API.Models
{
    public class NguoiDungNguyenLieu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int NguyenLieuId { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal SoLuong { get; set; }

        [Required, StringLength(50)]
        public string DonVi { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("NguyenLieuId")]
        public NguyenLieu NguyenLieu { get; set; }
    }
}
