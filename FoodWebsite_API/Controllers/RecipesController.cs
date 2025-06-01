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
        
        //Lấy danh sách công thức với tính năng lọc, sắp xếp và phân trang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetRecipes(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? specialtyId = null,
            [FromQuery] bool? isOriginal = null,
            [FromQuery] bool? isApproved = true,
            [FromQuery] int? maxPrepareTime = null,
            [FromQuery] int? maxCookingTime = null,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12)
        {
            var query = _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Name.Contains(searchTerm) ||
                                   (r.NamePlain != null && r.NamePlain.Contains(searchTerm)) ||
                                   (r.Description != null && r.Description.Contains(searchTerm)));
            }

            if (specialtyId.HasValue)
                query = query.Where(r => r.SpecialtyId == specialtyId.Value);

            if (isOriginal.HasValue)
                query = query.Where(r => r.IsOriginal == isOriginal.Value);

            if (isApproved.HasValue)
                query = query.Where(r => r.IsApproved == isApproved.Value);

            if (maxPrepareTime.HasValue)
                query = query.Where(r => r.PrepareTime <= maxPrepareTime.Value);

            if (maxCookingTime.HasValue)
                query = query.Where(r => r.CookingTime <= maxCookingTime.Value);

            // Sort
            query = sortBy.ToLower() switch
            {
                "name" => sortDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                "cookingtime" => sortDescending ? query.OrderByDescending(r => r.CookingTime) : query.OrderBy(r => r.CookingTime),
                "preparetime" => sortDescending ? query.OrderByDescending(r => r.PrepareTime) : query.OrderBy(r => r.PrepareTime),
                _ => sortDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
            };

            // Phân trang
            var totalItems = await query.CountAsync();
            var recipes = await query
                .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
            Response.Headers.Add("X-Page-Number", pageNumber.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)totalItems / pageSize)).ToString());

            return Ok(recipes);
        }

        // Lấy chi tiết công thức
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailDTO>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound(new { message = "Recipe not found" });

            var recipeDto = new RecipeDetailDTO
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
                RecipeSteps = recipe.RecipeSteps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new RecipeStepReadDTO
                    {
                        Id = s.Id,
                        RecipeId = s.RecipeId,
                        StepNumber = s.StepNumber,
                        Description = s.Description,
                        ImageUrl = s.ImageUrl
                    }).ToList(),
                RecipeIngredients = recipe.RecipeIngredients
                    .Select(ri => new RecipeIngredientReadDTO
                    {
                        RecipeId = ri.RecipeId,
                        IngredientId = ri.IngredientId,
                        IngredientName = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList()
            };

            return Ok(recipeDto);
        }

        [HttpPost]
        public async Task<ActionResult<RecipeDetailDTO>> CreateRecipe(RecipeCreateDTO createRecipeDto)
        {
            // Kiểm tra tồn tại đặc sản
            var specialtyExists = await _context.Specialties.AnyAsync(s => s.Id == createRecipeDto.SpecialtyId);
            if (!specialtyExists)
                return BadRequest(new { message = "Specialty not found" });

            // Kiểm tra tồn tại nguyên liệu
            var ingredientIds = createRecipeDto.RecipeIngredients.Select(ri => ri.IngredientId).ToList();
            var existingIngredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            var missingIngredients = ingredientIds.Except(existingIngredients).ToList();
            if (missingIngredients.Any())
                return BadRequest(new { message = $"Ingredients not found: {string.Join(", ", missingIngredients)}" });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recipe = new Recipe
                {
                    SpecialtyId = createRecipeDto.SpecialtyId,
                    Name = createRecipeDto.Name,
                    NamePlain = createRecipeDto.NamePlain,
                    IsOriginal = createRecipeDto.IsOriginal,
                    PrepareTime = createRecipeDto.PrepareTime,
                    CookingTime = createRecipeDto.CookingTime,
                    Description = createRecipeDto.Description,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                // Thêm bước nấu
                foreach (var stepDto in createRecipeDto.RecipeSteps)
                {
                    var step = new RecipeStep
                    {
                        RecipeId = recipe.Id,
                        StepNumber = stepDto.StepNumber,
                        Description = stepDto.Description,
                        ImageUrl = stepDto.ImageUrl
                    };
                    _context.RecipeSteps.Add(step);
                }

                // Thêm nguyên liệu của công thức
                foreach (var ingredientDto in createRecipeDto.RecipeIngredients)
                {
                    var recipeIngredient = new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = ingredientDto.IngredientId,
                        Quantity = ingredientDto.Quantity,
                        Unit = ingredientDto.Unit
                    };
                    _context.RecipeIngredients.Add(recipeIngredient);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetRecipe(recipe.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error creating recipe" });
            }
        }

        // PUT: api/recipes/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RecipeDetailDTO>> UpdateRecipe(int id, RecipeUpdateDTO updateRecipeDto)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound(new { message = "Recipe not found" });

            // Kiểm tra tồn tại đặc sản
            var specialtyExists = await _context.Specialties.AnyAsync(s => s.Id == updateRecipeDto.SpecialtyId);
            if (!specialtyExists)
                return BadRequest(new { message = "Specialty not found" });

            // Kiểm tra tồn tại nguyên liệu
            var ingredientIds = updateRecipeDto.RecipeIngredients.Select(ri => ri.IngredientId).ToList();
            var existingIngredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            var missingIngredients = ingredientIds.Except(existingIngredients).ToList();
            if (missingIngredients.Any())
                return BadRequest(new { message = $"Ingredients not found: {string.Join(", ", missingIngredients)}" });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                recipe.SpecialtyId = updateRecipeDto.SpecialtyId;
                recipe.Name = updateRecipeDto.Name;
                recipe.NamePlain = updateRecipeDto.NamePlain;
                recipe.IsOriginal = updateRecipeDto.IsOriginal;
                recipe.PrepareTime = updateRecipeDto.PrepareTime;
                recipe.CookingTime = updateRecipeDto.CookingTime;
                recipe.Description = updateRecipeDto.Description;
                recipe.UpdatedAt = DateTime.UtcNow;

                _context.RecipeSteps.RemoveRange(recipe.RecipeSteps);
                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

                foreach (var stepDto in updateRecipeDto.RecipeSteps)
                {
                    var step = new RecipeStep
                    {
                        RecipeId = recipe.Id,
                        StepNumber = stepDto.StepNumber,
                        Description = stepDto.Description,
                        ImageUrl = stepDto.ImageUrl
                    };
                    _context.RecipeSteps.Add(step);
                }

                foreach (var ingredientDto in updateRecipeDto.RecipeIngredients)
                {
                    var recipeIngredient = new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = ingredientDto.IngredientId,
                        Quantity = ingredientDto.Quantity,
                        Unit = ingredientDto.Unit
                    };
                    _context.RecipeIngredients.Add(recipeIngredient);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetRecipe(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error updating recipe" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found" });

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe deleted successfully" });
        }

        // POST: api/recipes/{id}/approve
        // Duyệt công thức
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found" });

            recipe.IsApproved = true;
            recipe.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe approved successfully" });
        }

        // POST: api/recipes/{id}/reject
        // Từ chối công thức
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found" });

            recipe.IsApproved = false;
            recipe.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe rejected successfully" });
        }

        // GET: api/recipes/popular
        // Lấy công thức nổi bật (theo lượt xem)
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetPopularRecipes([FromQuery] int limit = 10)
        {
            var popularRecipes = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.UserViewHistories)
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.UserViewHistories.Count)
                .ThenByDescending(r => r.CreatedAt)
            .Take(limit)
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

            return Ok(popularRecipes);
        }

        // GET: api/recipes/latest
        // Lấy công thức mới nhất
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetLatestRecipes([FromQuery] int limit = 10)
        {
            var latestRecipes = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
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

            return Ok(latestRecipes);
        }

        // GET: api/recipes/by-specialty/{specialtyId}
        // Lấy công thức theo đặc sản
        [HttpGet("by-specialty/{specialtyId}")]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetRecipesBySpecialty(
            int specialtyId,
            [FromQuery] int limit = 20)
        {
            var specialtyExists = await _context.Specialties.AnyAsync(s => s.Id == specialtyId);
            if (!specialtyExists)
                return NotFound(new { message = "Specialty not found" });

            var recipes = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .Where(r => r.SpecialtyId == specialtyId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
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

        // GET: api/recipes/quick-recipes
        // Lấy công thức nấu nhanh dưới 30 phút
        [HttpGet("quick-recipes")]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDTO>>> GetQuickRecipes([FromQuery] int maxTotalTime = 30)
        {
            var quickRecipes = await _context.Recipes
                .Include(r => r.Specialty)
                .Include(r => r.RecipeSteps)
                .Include(r => r.RecipeIngredients)
                .Where(r => r.IsApproved &&
                           (r.PrepareTime ?? 0) + r.CookingTime <= maxTotalTime)
                .OrderBy(r => (r.PrepareTime ?? 0) + r.CookingTime)
                .ThenByDescending(r => r.CreatedAt)
            .Take(20)
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

            return Ok(quickRecipes);
        }
    }
}
