using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.Province;
using FoodWebsite_API.DTOs.Recipe;
using FoodWebsite_API.DTOs.Specialty;
using FoodWebsite_API.DTOs.SpecialtyImages;
using FoodWebsite_API.Function;
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
        private readonly IWebHostEnvironment _env;

        public ProvincesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvinceReadDTO>>> GetAll([FromQuery] string? search, [FromQuery] bool? isActive)
        {
            var query = _context.Provinces.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = SlugHelper.RemoveDiacritics(search);
                query = query.Where(p => p.NamePlain.Contains(search)
                                       || p.RegionPlain.Contains(search));
                //|| (p.Description != null && p.Description.ToLower().Contains(search)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var provinces = await query
                .Select(p => new ProvinceReadDTO
                {
                    Id = p.Id,
                    Region = p.Region,
                    RegionPlain = p.RegionPlain,
                    Name = p.Name,
                    NamePlain = p.NamePlain,
                    Description = p.Description,
                    Version = p.Version,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return Ok(provinces);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProvinceReadDTO>> GetById(int id)
        {
            var province = await _context.Provinces
                .Where(p => p.Id == id)
                .Select(p => new ProvinceReadDTO
                {
                    Id = p.Id,
                    Region = p.Region,
                    RegionPlain = p.RegionPlain,
                    Name = p.Name,
                    NamePlain = p.NamePlain,
                    Description = p.Description,
                    Version = p.Version,
                    IsActive = p.IsActive,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (province == null)
                return NotFound();

            return Ok(province);
        }

        [HttpPost]
        public async Task<ActionResult<ProvinceReadDTO>> Create([FromBody] ProvinceCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var province = new Province
            {
                Region = dto.Region,
                RegionPlain = SlugHelper.RemoveDiacritics(dto.Region),
                Name = dto.Name,
                NamePlain = SlugHelper.RemoveDiacritics(dto.Name),
                Description = dto.Description,
                Version = dto.Version,
                ImageUrl = dto.ImageUrl,
                IsActive = true
            };

            _context.Provinces.Add(province);
            await _context.SaveChangesAsync();

            var resultDto = new ProvinceReadDTO
            {
                Id = province.Id,
                Region = province.Region,
                RegionPlain = province.RegionPlain,
                Name = province.Name,
                NamePlain = province.NamePlain,
                Description = province.Description,
                Version = province.Version,
                IsActive = province.IsActive,
                ImageUrl = province.ImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = province.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProvinceUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var province = await _context.Provinces.FindAsync(id);
            if (province == null)
                return NotFound();

            province.Region = dto.Region;
            province.RegionPlain = SlugHelper.RemoveDiacritics(dto.Region);
            province.Name = dto.Name;
            province.NamePlain = SlugHelper.RemoveDiacritics(dto.Name);
            province.Description = dto.Description;
            province.Version = dto.Version;
            province.IsActive = dto.IsActive;
            province.ImageUrl = dto.ImageUrl;

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

        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadProvinceImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Định dạng file không được hỗ trợ. Vui lòng chọn file ảnh (.jpg, .jpeg, .png, .webp).");

            var province = await _context.Provinces.FindAsync(id);
            if (province == null)
                return NotFound("Province không tồn tại.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "provinces");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(province.ImageUrl))
            {
                var oldFileName = province.ImageUrl.Replace("/images/provinces/", "");
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var provinceSlug = province.NamePlain?.ToLower() ?? "province";
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var uniqueFileName = $"{provinceSlug}-{timestamp}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            province.ImageUrl = $"/images/provinces/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { ImageUrl = province.ImageUrl });
        }

        [HttpGet("{id}/specialties")]
        public async Task<ActionResult<IEnumerable<SpecialtyDetailDTO>>> GetSpecialtiesByProvince(int id)
        {
            var provinceExists = await _context.Provinces.AnyAsync(p => p.Id == id);
            if (!provinceExists) return NotFound("Province not found");

            var specialties = await _context.Specialties
                .Where(s => s.ProvinceId == id)
                .Include(s => s.SpecialtyImages)
                .Include(s => s.Recipes)
                .Include(s => s.Ratings)
                .ToListAsync();

            var result = specialties.Select(s => new SpecialtyDetailDTO
            {
                Id = s.Id,
                Name = s.Name,
                NamePlain = s.NamePlain,
                Description = s.Description,
                ProvinceId = s.ProvinceId,
                ProvinceName = s.Province?.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                IsActive = s.IsActive,
                Province = new ProvinceReadDTO
                {
                    Id = id,
                    Name = s.Province?.Name ?? ""
                },
                ImageUrls = s.SpecialtyImages.Select(i => i.ImageUrl).ToList(),
                SpecialtyImages = s.SpecialtyImages.Select(i => new SpecialtyImagesReadDTO
                {
                    Id = i.Id,
                    SpecialtyId = i.Id,
                    ImageUrl = i.ImageUrl
                }).ToList(),
                Recipes = s.Recipes.Select(r => new RecipeDetailDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList()
            }).ToList();

            return Ok(result);
        }

    }
}
