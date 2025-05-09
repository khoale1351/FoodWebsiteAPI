using FoodWebsite_API.Data;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvincesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProvincesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetAll()
        {
            return await _context.Provinces.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Province>> GetById(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province == null) 
                return NotFound();
            return province;
        }

        [HttpPost]
        public async Task<ActionResult<Province>> Create(Province province)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set default values
            province.Version = province.Version > 0 ? province.Version : 1;
            province.IsActive = true;

            // Add the new province
            _context.Provinces.Add(province);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = province.Id }, province);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Province province)
        {
            if (id != province.Id)
            {
                return BadRequest("ID mismatch.");
            }

            // Check if the province exists
            var existingProvince = await _context.Provinces.FindAsync(id);
            if (existingProvince == null)
            {
                return NotFound();
            }

            // Update the properties
            existingProvince.Name = province.Name;
            existingProvince.Region = province.Region;
            existingProvince.Description = province.Description;
            existingProvince.Version = province.Version; // Ensure version is updated if necessary
            existingProvince.IsActive = province.IsActive;

            // Save changes
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province == null) 
                return NotFound();
            _context.Provinces.Remove(province);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
