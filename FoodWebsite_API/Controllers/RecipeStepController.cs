using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.RecipeStep;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers;

[ApiController]
[Route("api/recipes/{recipeId}/[controller]")]
public class RecipeStepController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private static readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public RecipeStepController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeStepReadDTO>>> GetSteps(int recipeId)
    {
        if (!await RecipeExists(recipeId))
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

    [HttpPost]
    public async Task<ActionResult<RecipeStepReadDTO>> CreateStep(int recipeId, [FromForm] RecipeStepCreateDTO dto)
    {
        if (!await RecipeExists(recipeId))
            return NotFound(new { message = "Recipe not found" });

        var image = dto.Image;
        if (image is not null && !IsValidImage(image, out var validationMsg))
            return BadRequest(new { message = validationMsg });

        var step = new RecipeStep
        {
            RecipeId = recipeId,
            StepNumber = dto.StepNumber,
            Description = dto.Description
        };

        _context.RecipeSteps.Add(step);
        await _context.SaveChangesAsync();

        if (image is not null)
        {
            step.ImageUrl = await SaveImageAsync(image, recipeId, step.Id, step.StepNumber);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetSteps), new { recipeId }, ToDTO(step));
    }

    [HttpPut("{stepId}")]
    public async Task<ActionResult<RecipeStepReadDTO>> UpdateStep(int recipeId, int stepId, [FromForm] RecipeStepUpdateDTO dto)
    {
        var step = await _context.RecipeSteps.FirstOrDefaultAsync(s => s.Id == stepId && s.RecipeId == recipeId);
        if (step is null)
            return NotFound(new { message = "Recipe step not found" });

        var image = dto.Image;
        if (image is not null && !IsValidImage(image, out var validationMsg))
            return BadRequest(new { message = validationMsg });

        string? oldImage = step.ImageUrl;
        step.StepNumber = dto.StepNumber;
        step.Description = dto.Description;

        if (image is not null)
        {
            step.ImageUrl = await SaveImageAsync(image, recipeId, step.Id, step.StepNumber);
            if (!string.IsNullOrEmpty(oldImage)) DeleteImage(oldImage);
        }

        await _context.SaveChangesAsync();
        return Ok(ToDTO(step));
    }

    [HttpDelete("{stepId}")]
    public async Task<IActionResult> DeleteStep(int recipeId, int stepId)
    {
        var step = await _context.RecipeSteps.FirstOrDefaultAsync(s => s.Id == stepId && s.RecipeId == recipeId);
        if (step is null)
            return NotFound(new { message = "Recipe step not found" });

        if (!string.IsNullOrEmpty(step.ImageUrl)) DeleteImage(step.ImageUrl);

        _context.RecipeSteps.Remove(step);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Recipe step deleted successfully" });
    }

    #region Helpers

    private async Task<bool> RecipeExists(int recipeId)
        => await _context.Recipes.AnyAsync(r => r.Id == recipeId);

    private static bool IsValidImage(IFormFile file, out string error)
    {
        error = string.Empty;

        if (file.Length > MaxFileSize)
        {
            error = "File exceeds max size of 5MB.";
            return false;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(ext))
        {
            error = $"Invalid file type. Allowed: {string.Join(", ", _allowedExtensions)}";
            return false;
        }

        return true;
    }

    private async Task<string> SaveImageAsync(IFormFile image, int recipeId, int stepId, int stepNumber)
    {
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "recipe-steps");
        Directory.CreateDirectory(uploadDir);

        var ext = Path.GetExtension(image.FileName);
        var guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
        var filename = $"{recipeId}-{stepId}.{stepNumber}-{guidPart}{ext}";
        var fullPath = Path.Combine(uploadDir, filename);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(stream);

        return $"/uploads/recipe-steps/{filename}";
    }

    private void DeleteImage(string imagePath)
    {
        try
        {
            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image deletion error: {ex.Message}");
        }
    }

    private static RecipeStepReadDTO ToDTO(RecipeStep step) => new()
    {
        Id = step.Id,
        RecipeId = step.RecipeId,
        StepNumber = step.StepNumber,
        Description = step.Description,
        ImageUrl = step.ImageUrl
    };

    #endregion
}
