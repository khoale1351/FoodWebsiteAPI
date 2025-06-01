using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.RecipeStep;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/[controller]")]
    public class RecipeStepController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipeStepController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/recipes/{recipeId}/steps
        // Lấy các bước nấu của công thức
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeStepReadDTO>>> GetRecipeSteps(int recipeId)
        {
            var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == recipeId);
            if (!recipeExists)
                return NotFound(new { message = "Recipe not found" });

            var steps = await _context.RecipeSteps
                .Where(s => s.RecipeId == recipeId)
                .OrderBy(s => s.StepNumber)
                .Select(s => new RecipeStepReadDTO
                {
                    Id = s.Id,
                    RecipeId = s.RecipeId,
                    StepNumber = s.StepNumber,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                })
                .ToListAsync();

            return Ok(steps);
        }

        // POST: api/recipes/{recipeId}/steps
        [HttpPost]
        public async Task<ActionResult<RecipeStepReadDTO>> CreateRecipeStep(int recipeId, RecipeStepCreateDTO createStepDto)
        {
            var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == recipeId);
            if (!recipeExists)
                return NotFound(new { message = "Recipe not found" });

            var step = new RecipeStep
            {
                RecipeId = recipeId,
                StepNumber = createStepDto.StepNumber,
                Description = createStepDto.Description,
                ImageUrl = createStepDto.ImageUrl
            };

            _context.RecipeSteps.Add(step);
            await _context.SaveChangesAsync();

            var stepDto = new RecipeStepReadDTO
            {
                Id = step.Id,
                RecipeId = step.RecipeId,
                StepNumber = step.StepNumber,
                Description = step.Description,
                ImageUrl = step.ImageUrl
            };

            return CreatedAtAction(nameof(GetRecipeSteps), new { recipeId }, stepDto);
        }

        // PUT: api/recipes/{recipeId}/steps/{stepId}
        [HttpPut("{stepId}")]
        public async Task<ActionResult<RecipeStepReadDTO>> UpdateRecipeStep(
            int recipeId,
            int stepId,
            RecipeStepUpdateDTO updateStepDto)
        {
            var step = await _context.RecipeSteps
                .FirstOrDefaultAsync(s => s.Id == stepId && s.RecipeId == recipeId);

            if (step == null)
                return NotFound(new { message = "Recipe step not found" });

            step.StepNumber = updateStepDto.StepNumber;
            step.Description = updateStepDto.Description;
            step.ImageUrl = updateStepDto.ImageUrl;

            await _context.SaveChangesAsync();

            var stepDto = new RecipeStepReadDTO
            {
                Id = step.Id,
                RecipeId = step.RecipeId,
                StepNumber = step.StepNumber,
                Description = step.Description,
                ImageUrl = step.ImageUrl
            };

            return Ok(stepDto);
        }

        // DELETE: api/recipes/{recipeId}/steps/{stepId}
        [HttpDelete("{stepId}")]
        public async Task<IActionResult> DeleteRecipeStep(int recipeId, int stepId)
        {
            var step = await _context.RecipeSteps
                .FirstOrDefaultAsync(s => s.Id == stepId && s.RecipeId == recipeId);

            if (step == null)
                return NotFound(new { message = "Recipe step not found" });

            _context.RecipeSteps.Remove(step);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe step deleted successfully" });
        }
    }
}
