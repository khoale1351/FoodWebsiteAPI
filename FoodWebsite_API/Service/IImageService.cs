namespace FoodWebsite_API.Service
{
    public interface IImageService
    {
        Task<string> SaveRecipeStepImageAsync(IFormFile image, int recipeId, int stepId, int stepNumber);
        void DeleteImage(string imagePath);
        (bool IsValid, string ErrorMessage) ValidateImage(IFormFile image);
    }
}
