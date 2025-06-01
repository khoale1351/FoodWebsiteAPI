using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.Province;
using FoodWebsite_API.DTOs.Rating;
using FoodWebsite_API.DTOs.Recipe;
using FoodWebsite_API.DTOs.RecipeIngredient;
using FoodWebsite_API.DTOs.RecipeStep;
using FoodWebsite_API.DTOs.Specialty;
using FoodWebsite_API.DTOs.SpecialtyImages;
using FoodWebsite_API.DTOs.UserViewHistory;
using FoodWebsite_API.Function;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpecialtiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialtyDetailDTO>>> GetAllOrByProvince([FromQuery] int? provinceId, [FromQuery] string? region)
        {
            IQueryable<Specialty> query = _context.Specialties
                .Include(s => s.Province)
                .Include(s => s.SpecialtyImages)
                .Include(s => s.Recipes)
                    .ThenInclude(r => r.RecipeSteps)
                .Include(s => s.Recipes)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(i => i.Ingredient)
                .Include(s => s.Ratings);

            if (provinceId.HasValue)
            {
                query = query.Where(s => s.ProvinceId == provinceId.Value);
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                string regionSearch = SlugHelper.RemoveDiacritics(region);
                query = query.Where(s => s.Province != null && s.Province.Region != null && s.Province.RegionPlain == regionSearch);
            }

            var specialties = await query.ToListAsync();

            var result = specialties.Select(s => new SpecialtyDetailDTO
            {
                Id = s.Id,
                Name = s.Name,
                NamePlain = s.NamePlain,
                Description = s.Description,
                ProvinceId = s.ProvinceId,
                ProvinceName = s.Province?.Name,
                Region = s.Province?.Region,
                RegionPlain = s.Province?.RegionPlain,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                IsActive = s.IsActive,
                Province = s.Province != null ? new ProvinceReadDTO
                {
                    Id = s.Province.Id,
                    Region = s.Province.Region,
                    Name = s.Province.Name,
                    Description = s.Province.Description,
                    Version = s.Province.Version,
                    IsActive = s.Province.IsActive,
                } : null,
                ImageUrls = s.SpecialtyImages.Select(i => i.ImageUrl).ToList(),
                SpecialtyImages = s.SpecialtyImages.Select(i => new SpecialtyImagesReadDTO
                {
                    Id = i.Id,
                    SpecialtyId = i.SpecialtyId,
                    ImageUrl = i.ImageUrl
                }).ToList(),
                Recipes = s.Recipes.Select(r => new RecipeDetailDTO
                {
                    Id = r.Id,
                    SpecialtyId = r.SpecialtyId,
                    Name = r.Name,
                    NamePlain = r.NamePlain,
                    Description = r.Description,
                    IsOriginal = r.IsOriginal,
                    PrepareTime = r.PrepareTime,
                    CookingTime = r.CookingTime,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    SpecialtyName = s.Name,
                    RecipeSteps = r.RecipeSteps.Select(rs => new RecipeStepReadDTO
                    {
                        Id = rs.Id,
                        RecipeId = rs.RecipeId,
                        StepNumber = rs.StepNumber,
                        Description = rs.Description,
                        ImageUrl = rs.ImageUrl
                    }).OrderBy(rs => rs.StepNumber).ToList(),
                    RecipeIngredients = r.RecipeIngredients.Select(ri => new RecipeIngredientReadDTO
                    {
                        RecipeId = ri.RecipeId,
                        IngredientName = ri.Ingredient?.Name ?? string.Empty,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList(),
                }).ToList()             
            }).ToList();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialtyDetailDTO>> GetById(int id)
        {
            var s = await _context.Specialties
                .Include(s => s.Province)
                .Include(s => s.SpecialtyImages)
                .Include(s => s.Recipes)
                    .ThenInclude(r => r.RecipeSteps)
                .Include(s => s.Recipes)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(i => i.Ingredient)
                .Include(s => s.Ratings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (s == null) return NotFound();

            var dto = new SpecialtyDetailDTO
            {
                Id = s.Id,
                Name = s.Name,
                NamePlain = s.NamePlain,
                Description = s.Description,
                ProvinceId = s.ProvinceId,
                ProvinceName = s.Province?.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                IsActive = s.IsActive,
                Province = s.Province != null ? new ProvinceReadDTO
                {
                    Id = s.Province.Id,
                    Name = s.Province.Name
                } : null,
                ImageUrls = s.SpecialtyImages.Select(i => i.ImageUrl).ToList(),
                SpecialtyImages = s.SpecialtyImages.Select(i => new SpecialtyImagesReadDTO
                {
                    Id = i.Id,
                    SpecialtyId = i.SpecialtyId,
                    ImageUrl = i.ImageUrl
                }).ToList(),
                Recipes = s.Recipes.Select(r => new RecipeDetailDTO
                {
                    Id = r.Id,
                    SpecialtyId = r.SpecialtyId,
                    Name = r.Name,
                    NamePlain = r.NamePlain,
                    Description = r.Description,
                    IsOriginal = r.IsOriginal,
                    PrepareTime = r.PrepareTime,
                    CookingTime = r.CookingTime,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    SpecialtyName = s.Name,
                    RecipeSteps = r.RecipeSteps.Select(rs => new RecipeStepReadDTO
                    {
                        Id = rs.Id,
                        RecipeId = rs.RecipeId,
                        StepNumber = rs.StepNumber,
                        Description = rs.Description,
                        ImageUrl = rs.ImageUrl
                    }).OrderBy(rs => rs.StepNumber).ToList(),
                    RecipeIngredients = r.RecipeIngredients.Select(ri => new RecipeIngredientReadDTO
                    {
                        RecipeId = ri.RecipeId,
                        IngredientName = ri.Ingredient?.Name ?? string.Empty,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList(),
                }).ToList()
            };

            return Ok(dto);
        }


        [HttpPost]
        public async Task<ActionResult<Specialty>> Create(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = specialty.Id }, specialty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Specialty specialty)
        {
            if (id != specialty.Id)
                return BadRequest();
            _context.Entry(specialty).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Specialties.Any(e => e.Id == specialty.Id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
                return NotFound();
            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSpecialty([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Từ khoá tìm kiếm không hợp lệ.");

            string normalizedQuery = query.Trim().RemoveDiacritics();
            string rawQueryLower = query.Trim().ToLowerInvariant();

            var result = await _context.Specialties
                .Include(s => s.SpecialtyImages)
                .Where(s => s.Name.ToLower().Contains(rawQueryLower) || (s.NamePlain != null && s.NamePlain.Contains(normalizedQuery)))
                .Select(s => new SpecialtySearchResultDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Images = s.SpecialtyImages.FirstOrDefault().ImageUrl
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<SpecialtyDetailDTO>> GetSpecialtyWithDetails(int id)
        {
            var specialty = await _context.Specialties
                .Include(s => s.Province)
                .Include(s => s.SpecialtyImages)
                .Include(s => s.Recipes)
                    .ThenInclude(r => r.RecipeSteps)
                .Include(s => s.Recipes)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(i => i.Ingredient)
                .Include(s => s.Ratings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialty == null)
                return NotFound();

            var dto = new SpecialtyDetailDTO
            {
                Id = specialty.Id,
                Name = specialty.Name,
                NamePlain = specialty.NamePlain,
                Description = specialty.Description,
                ProvinceId = specialty.ProvinceId,
                ProvinceName = specialty.Province?.Name,
                CreatedAt = specialty.CreatedAt,
                UpdatedAt = specialty.UpdatedAt,
                IsActive = specialty.IsActive,

                ImageUrls = specialty.SpecialtyImages
                    .Where(img => !string.IsNullOrEmpty(img.ImageUrl))
                    .Select(img => img.ImageUrl!)
                    .ToList(),

                SpecialtyImages = specialty.SpecialtyImages.Select(img => new SpecialtyImagesReadDTO
                {
                    Id = img.Id,
                    SpecialtyId = img.SpecialtyId,
                    ImageUrl = img.ImageUrl
                }).ToList(),

                Ratings = specialty.Ratings.Select(r => new RatingReadDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User.UserName,
                    SpecialtyId = r.SpecialtyId,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList(),

                Recipes = specialty.Recipes.Select(r => new RecipeDetailDTO
                {
                    Id = r.Id,
                    SpecialtyId = r.SpecialtyId,
                    Name = r.Name,
                    NamePlain = r.NamePlain,
                    Description = r.Description,
                    IsOriginal = r.IsOriginal,
                    PrepareTime = r.PrepareTime,
                    CookingTime = r.CookingTime,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    SpecialtyName = specialty.Name,
                    RecipeSteps = r.RecipeSteps.Select(rs => new RecipeStepReadDTO
                    {
                        Id = rs.Id,
                        RecipeId = rs.RecipeId,
                        StepNumber = rs.StepNumber,
                        Description = rs.Description,
                        ImageUrl = rs.ImageUrl
                    }).OrderBy(rs => rs.StepNumber).ToList(),
                    RecipeIngredients = r.RecipeIngredients.Select(ri => new RecipeIngredientReadDTO
                    {
                        RecipeId = ri.RecipeId,
                        IngredientName = ri.Ingredient?.Name ?? string.Empty,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList(),
                }).ToList()
            };

            return Ok(dto);
        }
    }
}
