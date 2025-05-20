using FoodWebsite_API.Data;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetAll()
        {
            return await _context.Ratings.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Rating>> GetById(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            return rating == null ? NotFound() : rating;
        }
//create
        [HttpPost]
        public async Task<ActionResult<Rating>> CreateRating(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }
//update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, Rating rating)
        {
            if (id != rating.Id) return BadRequest();
            _context.Entry(rating).State = EntityState.Modified;
            try 
            { 
                await _context.SaveChangesAsync(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ratings.Any(e => e.Id == id)) 
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null) 
                return NotFound();
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("user-review")]
        public async Task<IActionResult> PostReview(Rating rating)
        {
            rating.UserId = _userManager.GetUserId(User);
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return Ok(rating);
        }

        [HttpGet("specialty/{specialtyId}")]
        public async Task<IActionResult> GetReviewsByDacSan(int specialtyId)
        {
            var list = await _context.Ratings
                .Where(r => r.SpecialtyId == specialtyId)
                .Include(r => r.User)
                .ToListAsync();
            return Ok(list);
        }
    }
}
