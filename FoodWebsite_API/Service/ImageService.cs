namespace FoodWebsite_API.Service
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public (bool IsValid, string ErrorMessage) ValidateImage(IFormFile image)
        {
            if (image.Length > MaxFileSize)
                return (false, $"File size must not exceed {MaxFileSize / 1024 / 1024}MB");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(extension))
                return (false, $"Only image files with extensions {string.Join(", ", _allowedImageExtensions)} are allowed");

            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp" };
            if (!allowedContentTypes.Contains(image.ContentType.ToLowerInvariant()))
                return (false, "Invalid image content type");

            return (true, string.Empty);
        }

        public async Task<string> SaveRecipeStepImageAsync(IFormFile image, int recipeId, int stepId, int stepNumber)
        {
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "recipe-steps");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var extension = Path.GetExtension(image.FileName);
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            var fileName = $"{recipeId}-{stepId}.{stepNumber}-{uniqueId}{extension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"/uploads/recipe-steps/{fileName}";
        }

        public void DeleteImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // Log error - use proper logging framework
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }
    }
}
