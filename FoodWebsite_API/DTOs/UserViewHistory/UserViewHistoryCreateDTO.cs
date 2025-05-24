using System.ComponentModel.DataAnnotations;

namespace FoodWebsite_API.DTOs.UserViewHistory
{
    public class UserViewHistoryCreateDTO : IValidatableObject
    {
        public int? SpecialtyId { get; set; }
        public int? RecipeId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((SpecialtyId == null && RecipeId == null) || (SpecialtyId != null && RecipeId != null))
            {
                yield return new ValidationResult(
                    "Chỉ được chọn 1 trong 2: SpecialtyId hoặc RecipeId.",
                    new[] { nameof(SpecialtyId), nameof(RecipeId) });
            }
        }
    }
}
