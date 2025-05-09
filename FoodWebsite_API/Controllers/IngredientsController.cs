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

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id) 
                return BadRequest();
            _context.Entry(ingredient).State = EntityState.Modified;
            try 
            { 
                await _context.SaveChangesAsync(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ingredients.Any(e => e.Id == id)) 
                    return NotFound();
                throw;
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
