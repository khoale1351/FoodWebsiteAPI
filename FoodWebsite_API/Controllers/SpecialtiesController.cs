using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.Specialty;
using FoodWebsite_API.Function;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpecialtiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetAll()
        {
            return await _context.Specialties.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Specialty>> GetById(int id)
        {
            var specialty = await _context.Specialties.Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id);
            if (specialty == null) 
                return NotFound();
            return specialty;
        }

        [HttpGet("../Provinces/{provinceId}/Specialties")]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialtiesByProvinceId (int provinceId)
        {
            return await _context.Specialties.Where(s => s.ProvinceId == provinceId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Specialty>> Create(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = specialty.Id }, specialty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Specialty specialty)
        {
            if (id != specialty.Id) 
                return BadRequest();
            _context.Entry(specialty).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Specialties.Any(e  => e.Id == specialty.Id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) 
                return NotFound();
            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Specialty>>> SearchSpecialty([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Thiếu từ khóa");

            query = SlugHelper.RemoveDiacritics(query).ToLower();
            var result = await _context.Specialties
                .Where(s => s.NamePlain.Contains(query))
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<SpecialtyDetailDTO>> GetSpecialtyWithImages(int id)
        {
            var specialty = await _context.Specialties
                .Include(s => s.Province)
                .Include(s => s.SpecialtyImages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (specialty == null)
                return NotFound();

            var dto = new SpecialtyDetailDTO
            {
                Id = specialty.Id,
                Name = specialty.Name,
                Description = specialty.Description,
                ProvinceName = specialty.Province?.Name,
                ImageUrls = specialty.SpecialtyImages.Select(img => img.ImageUrl).ToList()
            };

            return Ok(dto);
        }
    }
}
