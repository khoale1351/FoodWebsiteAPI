using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.UserViewHistory;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodWebsite_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserViewHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserViewHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserViewHistoryReadDTO>>> GetUserHistory(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId)
            {
                return Forbid();
            }

            var histories = await _context.UserViewHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ViewedAt)
                .Include(h => h.Specialty)
                .Include(h => h.Recipe)
                .Select(h => new UserViewHistoryReadDTO
                {
                    Id = h.Id,
                    UserId = h.UserId,
                    SpecialtyName = h.Specialty != null ? h.Specialty.Name : null,
                    RecipeName = h.Recipe != null ? h.Recipe.Name : null,
                    ViewedAt = h.ViewedAt
                })
                .ToListAsync();

            return Ok(histories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserViewHistoryReadDTO>> GetById(int id)
        {
            var history = await _context.UserViewHistories
                .Include(h => h.User)
                .Include(h => h.Specialty)
                .Include(h => h.Recipe)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (history == null) return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (history.UserId != currentUserId)
                return Forbid();

            return new UserViewHistoryReadDTO
            {
                Id = history.Id,
                UserId = history.UserId,
                UserName = history.User?.UserName,
                SpecialtyId = history.SpecialtyId,
                SpecialtyName = history.Specialty?.Name,
                RecipeId = history.RecipeId,
                RecipeName = history.Recipe?.Name,
                ViewedAt = history.ViewedAt
            };
        }

        [HttpPost]
        public async Task<ActionResult> CreateHistory([FromBody] UserViewHistoryCreateDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var history = new UserViewHistory
            {
                UserId = userId,
                SpecialtyId = dto.SpecialtyId,
                RecipeId = dto.RecipeId,
                ViewedAt = DateTime.Now
            };
            _context.UserViewHistories.Add(history);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            var specialty = await _context.Specialties.FindAsync(dto.SpecialtyId);
            var recipe = await _context.Recipes.FindAsync(dto.RecipeId);
            var readDto = new UserViewHistoryReadDTO
            {
                Id = history.Id,
                UserId = history.UserId,
                UserName = user?.UserName,
                SpecialtyId = history.SpecialtyId,
                SpecialtyName = specialty?.Name,
                RecipeId = history.RecipeId,
                RecipeName = recipe?.Name,
                ViewedAt = history.ViewedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = history.Id }, readDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var history = await _context.UserViewHistories.FindAsync(id);
            if (history == null) return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (history.UserId != currentUserId)
                return Forbid();

            _context.UserViewHistories.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }       
    }
}
