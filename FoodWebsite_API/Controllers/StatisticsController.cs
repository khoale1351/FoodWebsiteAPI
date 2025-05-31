using FoodWebsite_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("top-specialties")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopSpecialties(int top = 5, int days = 7)
        {
            var sinceDate = DateTime.Now.AddDays(-days);
            var topSpecialties = await _context.UserViewHistories
                .Where(h => h.SpecialtyId != null && h.ViewedAt >= sinceDate)
                .GroupBy(h => h.SpecialtyId)
                .Select(g => new
                {
                    SpecialtyId = g.Key,
                    ViewCount = g.Count()
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(top)
                .Join(_context.Specialties.Include(s => s.SpecialtyImages),
                      g => g.SpecialtyId,
                      s => s.Id,
                      (g, s) => new
                      {
                          SpecialtyId = g.SpecialtyId,
                          SpecialtyName = s.Name,
                          ViewCount = g.ViewCount,
                          Image = s.SpecialtyImages.FirstOrDefault().ImageUrl
                      })
                .ToListAsync();

            return Ok(topSpecialties);
        }

        [HttpGet("top-recipes")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopRecipes(int top = 5, int days = 7)
        {
            var sinceDate = DateTime.Now.AddDays(-days);
            var topRecipes = await _context.UserViewHistories
                .Where(h => h.RecipeId != null && h.ViewedAt >= sinceDate)
                .GroupBy(h => h.RecipeId)
                .Select(g => new
                {
                    RecipeId = g.Key,
                    ViewCount = g.Count()
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(top)
                .Join(_context.Recipes,
                      g => g.RecipeId,
                      r => r.Id,
                      (g, r) => new
                      {
                          RecipeId = g.RecipeId,
                          RecipeName = r.Name,
                          ViewCount = g.ViewCount
                      })
                .ToListAsync();

            return Ok(topRecipes);
        }
    }
}
