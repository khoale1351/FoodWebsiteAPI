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

        [HttpGet("{userId}")]
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

        [HttpPost]
        public async Task<ActionResult> CreateHistory([FromBody] UserViewHistoryCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var history = new UserViewHistory
            {
                UserId = dto.UserId,
                SpecialtyId = dto.SpecialtyId,
                RecipeId = dto.RecipeId,
                ViewedAt = DateTime.Now
            };

            _context.UserViewHistories.Add(history);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserHistory), new { userId = dto.UserId }, null);
        }

        [HttpGet("top-specialties")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopSpecialties(int top = 5)
        {
            var topSpecialties = await _context.UserViewHistories
                .Where(h => h.SpecialtyId != null)
                .GroupBy(h => h.SpecialtyId)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => new
                {
                    SpecialtyId = g.Key,
                    Count = g.Count(),
                    Name = _context.Specialties.Where(s => s.Id == g.Key).Select(s => s.Name).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(topSpecialties);
        }

        [HttpGet("top-recipes")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopRecipes(int top = 5)
        {
            var topRecipes = await _context.UserViewHistories
                .Where(h => h.RecipeId != null)
                .GroupBy(h => h.RecipeId)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => new
                {
                    RecipeId = g.Key,
                    Count = g.Count(),
                    Name = _context.Recipes.Where(r => r.Id == g.Key).Select(r => r.Name).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(topRecipes);
        }
    }
}
