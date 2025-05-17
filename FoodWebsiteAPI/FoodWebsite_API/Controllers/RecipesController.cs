using FoodWebsite_API.Data;
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
        public RecipesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            return await _context.Recipes.ToListAsync();
        } 

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var rec = await _context.Recipes.FindAsync(id);
            return rec == null ? NotFound() : rec;
        }

    //create
        [HttpPost]
        public async Task<ActionResult<Recipe>> Create(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
        }

        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Recipe recipe)
        {
            if (id != recipe.Id) return BadRequest();
            _context.Entry(recipe).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Recipes.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var rec = await _context.Recipes.FindAsync(id);
            if (rec == null) 
                return NotFound();
            _context.Recipes.Remove(rec);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("specialty/{specialtyId}")]
        public async Task<IActionResult> GetByDacSan(int specialtyId)
        {
            var congThucs = await _context.Recipes
                .Where(ct => ct.SpecialtyId == specialtyId && ct.IsApproved)
                .Include(ct => ct.RecipeIngredients)
                .ThenInclude(nl => nl.Ingredient)
                .ToListAsync();
            return Ok(congThucs);
        }

        private bool CongThucExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
