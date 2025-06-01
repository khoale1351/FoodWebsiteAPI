using FoodWebsite_API.DTOs.Province;
using FoodWebsite_API.DTOs.Rating;
using FoodWebsite_API.DTOs.Recipe;
using FoodWebsite_API.DTOs.SpecialtyImages;
using FoodWebsite_API.DTOs.UserViewHistory;

namespace FoodWebsite_API.DTOs.Specialty
{
    public class SpecialtyDetailDTO
    {
        public int Id { get; set; }
        public int ProvinceId { get; set; }

        public string Region { get; set; } = null!;

        public string? RegionPlain { get; set; }

        public string Name { get; set; } = null!;
        public string? NamePlain { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? ProvinceName { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        // Thông tin tỉnh/thành của đặc sản
        public ProvinceReadDTO? Province { get; set; }

        // Danh sách hình ảnh
        public List<SpecialtyImagesReadDTO> SpecialtyImages { get; set; } = new();

        // Danh sách công thức nấu ăn liên quan
        public List<RecipeDetailDTO> Recipes { get; set; } = new();

        // Danh sách đánh giá người dùng
        public List<RatingReadDTO> Ratings { get; set; } = new();
    }
}
