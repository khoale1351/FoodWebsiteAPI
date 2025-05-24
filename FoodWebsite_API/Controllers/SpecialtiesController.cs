using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.Rating;
using FoodWebsite_API.DTOs.Recipe;
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
        public async Task<ActionResult<IEnumerable<Specialty>>> GetAll()
        {
            return await _context.Specialties.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Specialty>> GetById(int id)
        {
            var specialty = await _context.Specialties.Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id);
            if (specialty == null) 
                return NotFound();
            return specialty;
        }

        [HttpGet("../Provinces/{provinceId}/Specialties")]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialtiesByProvinceId (int provinceId)
        {
            return await _context.Specialties.Where(s => s.ProvinceId == provinceId).ToListAsync();
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
                if (!_context.Specialties.Any(e  => e.Id == specialty.Id))
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
        public async Task<ActionResult<IEnumerable<Specialty>>> SearchSpecialty([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Thiếu từ khóa");

            var result = await _context.Specialties
                .Where(s => s.Name.Contains(query.ToLower()) || s.NamePlain.Contains(SlugHelper.RemoveDiacritics(query)))
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

                Recipes = specialty.Recipes.Select(recipe => new RecipeReadDTO
                {
                    Id = recipe.Id,
                    SpecialtyId = recipe.SpecialtyId,
                    Name = recipe.Name,
                    NamePlain = recipe.NamePlain,
                    IsOriginal = recipe.IsOriginal,
                    PrepareTime = recipe.PrepareTime,
                    CookingTime = recipe.CookingTime,
                    Description = recipe.Description,
                    IsApproved = recipe.IsApproved,
                    CreatedAt = recipe.CreatedAt,
                    UpdatedAt = recipe.UpdatedAt,
                    FavoriteCount = recipe.UserFavoriteRecipes.Count,
                    ViewCount = recipe.UserViewHistories.Count
                }).ToList()
            };

            return Ok(dto);
        }
    }
}
