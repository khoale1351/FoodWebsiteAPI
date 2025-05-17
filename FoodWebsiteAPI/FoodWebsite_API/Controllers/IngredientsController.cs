using FoodWebsite_API.Data;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IngredientsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            return await _context.Ingredients.Where(nl => nl.IsActive).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var nl = await _context.Ingredients.FindAsync(id);
            return nl == null ? NotFound() : nl;
        }


        // Create
        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            if (ingredient == null)
                return BadRequest("Ingredient cannot be null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ingredient.IsActive = true; // If applying soft-delete

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ingredient ingredient)
        {
            if (ingredient == null)
                return BadRequest("Ingredient cannot be null.");

            if (id != ingredient.Id)
                return BadRequest("Ingredient ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ingredients.Any(e => e.Id == id))
                    return NotFound();
                throw; // Re-throw the exception for further handling
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var nl = await _context.Ingredients.FindAsync(id);
            if (nl == null) 
                return NotFound();
            _context.Ingredients.Remove(nl);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
