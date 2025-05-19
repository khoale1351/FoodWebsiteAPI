using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FoodWebsite_API.Function
{
    public static class SlugHelper
    {
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            result = Regex.Replace(result, @"[^\w\s]", ""); 
            result = Regex.Replace(result, @"\s+", "");

            return result;
        }

        public static string ToSlug(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            //1. Bỏ dấu, chuyển về chữ thường
            var noDiacritics = text.RemoveDiacritics().ToLowerInvariant();
            //2. Thay ký tự không hợp lệ thành dấu "-"
            var replaced = Regex.Replace(noDiacritics, @"[^a-z0-9\s-]", "-");
            //3. Xoá ký tự trùng lặp
            var collapsed = Regex.Replace(replaced, @"[\s-]+", "-".Trim('-'));
            // Trim hyphens from the start and end
            return collapsed;
        }
    }
}
