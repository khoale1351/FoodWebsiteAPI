using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.Recipe;
using FoodWebsite_API.DTOs.RecipeIngredient;
using FoodWebsite_API.DTOs.RecipeStep;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetRecipes([FromQuery] RecipeFilterDTO filter)
        {
            var query = _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(r => r.Name.Contains(filter.SearchTerm) ||
                                         (r.NamePlain != null && r.NamePlain.Contains(filter.SearchTerm)) ||
                                         (r.Description != null && r.Description.Contains(filter.SearchTerm)));
            }

            if (filter.SpecialtyId.HasValue)
                query = query.Where(r => r.SpecialtyId == filter.SpecialtyId);

            if (filter.IsOriginal.HasValue)
                query = query.Where(r => r.IsOriginal == filter.IsOriginal);

            if (filter.IsApproved.HasValue)
                query = query.Where(r => r.IsApproved == filter.IsApproved);

            if (filter.MaxPrepareTime.HasValue)
                query = query.Where(r => r.PrepareTime <= filter.MaxPrepareTime);

            if (filter.MaxCookingTime.HasValue)
                query = query.Where(r => r.CookingTime <= filter.MaxCookingTime);

            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                "cookingtime" => filter.SortDescending ? query.OrderByDescending(r => r.CookingTime) : query.OrderBy(r => r.CookingTime),
                "preparetime" => filter.SortDescending ? query.OrderByDescending(r => r.PrepareTime) : query.OrderBy(r => r.PrepareTime),
                _ => filter.SortDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var recipes = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new RecipeSummaryDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    NamePlain = r.NamePlain,
                    IsOriginal = r.IsOriginal,
                    PrepareTime = r.PrepareTime,
                    CookingTime = r.CookingTime,
                    Description = r.Description,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt,
                    SpecialtyName = r.Specialty.Name,
                    TotalSteps = r.RecipeSteps.Count,
                    TotalIngredients = r.RecipeIngredients.Count
                })
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalItems.ToString());
            Response.Headers.Add("X-Page-Number", filter.PageNumber.ToString());
            Response.Headers.Add("X-Page-Size", filter.PageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)totalItems / filter.PageSize)).ToString());

            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailDTO>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null) return NotFound(new { message = "Recipe not found" });

            var favoriteCount = await _context.UserFavoriteRecipes.CountAsync(f => f.RecipeId == id);
            var viewCount = await _context.UserViewHistories.CountAsync(v => v.RecipeId == id);

            return Ok(new RecipeDetailDTO
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
                SpecialtyName = recipe.Specialty.Name,
                FavoriteCount = favoriteCount,
                ViewCount = viewCount,
                RecipeSteps = recipe.RecipeSteps.OrderBy(s => s.StepNumber).Select(s => new RecipeStepReadDTO
                {
                    Id = s.Id,
                    RecipeId = s.RecipeId,
                    StepNumber = s.StepNumber,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                }).ToList(),
                RecipeIngredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientReadDTO
                {
                    RecipeId = ri.RecipeId,
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient.Name,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                }).ToList()
            });
        }


        [HttpPost]
        public async Task<ActionResult> PostRecipe(RecipeCreateDTO dto)
        {
            var recipe = new Recipe
            {
                SpecialtyId = dto.SpecialtyId,
                Name = dto.Name,
                NamePlain = dto.NamePlain,
                IsOriginal = dto.IsOriginal,
                PrepareTime = dto.PrepareTime,
                CookingTime = dto.CookingTime,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsApproved = false
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            foreach (var stepDto in dto.RecipeSteps)
            {
                _context.RecipeSteps.Add(new RecipeStep
                {
                    RecipeId = recipe.Id,
                    StepNumber = stepDto.StepNumber,
                    Description = stepDto.Description,
                    ImageUrl = stepDto.ImageUrl
                });
            }

            foreach (var ingDto in dto.RecipeIngredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingDto.IngredientId,
                    Quantity = ingDto.Quantity,
                    Unit = ingDto.Unit
                });
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, RecipeCreateDTO dto)
        {
            var recipe = await _context.Recipes.Include(r => r.RecipeSteps).Include(r => r.RecipeIngredients).FirstOrDefaultAsync(r => r.Id == id);
            if (recipe == null) return NotFound();

            recipe.SpecialtyId = dto.SpecialtyId;
            recipe.Name = dto.Name;
            recipe.NamePlain = dto.NamePlain;
            recipe.IsOriginal = dto.IsOriginal;
            recipe.PrepareTime = dto.PrepareTime;
            recipe.CookingTime = dto.CookingTime;
            recipe.Description = dto.Description;
            recipe.UpdatedAt = DateTime.UtcNow;

            _context.RecipeSteps.RemoveRange(recipe.RecipeSteps);
            _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
            await _context.SaveChangesAsync();

            foreach (var stepDto in dto.RecipeSteps)
            {
                _context.RecipeSteps.Add(new RecipeStep
                {
                    RecipeId = recipe.Id,
                    StepNumber = stepDto.StepNumber,
                    Description = stepDto.Description,
                    ImageUrl = stepDto.ImageUrl
                });
            }

            foreach (var ingDto in dto.RecipeIngredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingDto.IngredientId,
                    Quantity = ingDto.Quantity,
                    Unit = ingDto.Unit
                });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            recipe.IsApproved = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            recipe.IsApproved = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("quick")]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetQuickRecipes()
        {
            int maxTotalTime = 30;
            var recipes = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .Where(r => (r.PrepareTime ?? 0) + r.CookingTime <= maxTotalTime && r.IsApproved)
                .Select(r => new RecipeSummaryDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    NamePlain = r.NamePlain,
                    IsOriginal = r.IsOriginal,
                    PrepareTime = r.PrepareTime,
                    CookingTime = r.CookingTime,
                    Description = r.Description,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt,
                    SpecialtyName = r.Specialty.Name,
                    TotalSteps = r.RecipeSteps.Count,
                    TotalIngredients = r.RecipeIngredients.Count
                })
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("original")]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetOriginalRecipes()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .Where(r => r.IsOriginal && r.IsApproved)
                .Select(r => new RecipeSummaryDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    NamePlain = r.NamePlain,
                    IsOriginal = r.IsOriginal,
                    PrepareTime = r.PrepareTime,
                    CookingTime = r.CookingTime,
                    Description = r.Description,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt,
                    SpecialtyName = r.Specialty.Name,
                    TotalSteps = r.RecipeSteps.Count,
                    TotalIngredients = r.RecipeIngredients.Count
                })
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpPost("{id}/upload-step-image")]
        public async Task<IActionResult> UploadStepImage(int id, int stepNumber, IFormFile file)
        {
            var recipeStep = await _context.RecipeSteps
                .FirstOrDefaultAsync(rs => rs.RecipeId == id && rs.StepNumber == stepNumber);

            if (recipeStep == null) return NotFound();

            var fileName = $"step_{id}_{stepNumber}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine("wwwroot/images/steps", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            recipeStep.ImageUrl = $"/images/steps/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { imageUrl = recipeStep.ImageUrl });
        }
    }
}
