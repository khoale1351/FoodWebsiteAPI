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
    public class UserIngredientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserIngredientsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserIngredient>>> GetAll()
        {
            return await _context.UserIngredients
                .Include(x => x.Ingredient)
                .Include(x => x.User)
                .ToListAsync();
        }

        // GET: api/NguoiDungNguyenLieu/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<UserIngredient>>> GetByUser(string userId)
        {
            return await _context.UserIngredients
                .Where(x => x.UserId == userId)
                .Include(x => x.Ingredient)
                .ToListAsync();
        }

        // POST: api/NguoiDungNguyenLieu
        [HttpPost]
        public async Task<ActionResult<UserIngredient>> Post(UserIngredient ndnl)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.UserIngredients.Add(ndnl);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByUser), new { userId = ndnl.UserId }, ndnl);
        }

        // PUT: api/NguoiDungNguyenLieu/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UserIngredient ndnl)
        {
            if (id != ndnl.Id)
                return BadRequest();

            _context.Entry(ndnl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserIngredients.Any(x => x.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/NguoiDungNguyenLieu/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.UserIngredients.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.UserIngredients.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // API GỢI Ý MÓN ĂN từ nguyên liệu người dùng có
        
    }
}
