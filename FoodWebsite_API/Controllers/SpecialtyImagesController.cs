using FoodWebsite_API.Data;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtyImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SpecialtyImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialtyImage>>> GetImages([FromQuery] int specialtyId)
        {
            return await _context.SpecialtyImages.Where(img => img.SpecialtyId == specialtyId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SpecialtyImage>> UploadImage(SpecialtyImage image)
        {
            _context.SpecialtyImages.Add(image);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetImages), new { specialtyId = image.SpecialtyId }, image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.SpecialtyImages.FindAsync(id);
            if (image == null)
                return NotFound();

            _context.SpecialtyImages.Remove(image);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
