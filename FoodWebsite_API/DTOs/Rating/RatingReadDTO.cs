namespace FoodWebsite_API.DTOs.Rating
{
    public class RatingReadDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int SpecialtyId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Tuỳ bạn muốn hiển thị tên người dùng hay không
        public string? UserName { get; set; }
    }
}
