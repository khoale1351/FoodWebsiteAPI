using FoodWebsite_API.Data;
using FoodWebsite_API.DTOs.SpecialtyImages;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] SpecialtyImageUpoadDTO dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("File không hợp lệ");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "specialties");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(dto.ImageFile.FileName).ToLower();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
                return BadRequest("Chỉ cho phép định dạng .jpg, .jpeg, .png, .webp");

            var originalName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName);
            var safeName = Regex.Replace(originalName.ToLower(), "[^a-z0-9]+", "-").Trim('-');

            var fileName = $"{safeName}-{DateTime.UtcNow.Ticks}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            var image = new SpecialtyImage
            {
                SpecialtyId = dto.SpecialtyId,
                ImageUrl = $"/images/specialties/{fileName}"
            };

            _context.SpecialtyImages.Add(image);
            await _context.SaveChangesAsync();

            return Ok(image);
        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleImages([FromForm] int specialtyId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Không có ảnh nào được chọn.");

            var uploadedImages = new List<SpecialtyImage>();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "specialties");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
                        continue;

                    var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                    var safeName = Regex.Replace(originalName.ToLower(), "[^a-z0-9]+", "-").Trim('-');

                    var fileName = $"{safeName}-{DateTime.UtcNow.Ticks}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var image = new SpecialtyImage
                    {
                        SpecialtyId = specialtyId,
                        ImageUrl = $"/images/specialties/{fileName}"
                    };

                    uploadedImages.Add(image);
                }
            }

            _context.SpecialtyImages.AddRange(uploadedImages);
            await _context.SaveChangesAsync();

            return Ok(uploadedImages);
        }

        [HttpPost("upload-multi-specialties")]
        public async Task<IActionResult> UploadMultipleSpecialtyImages([FromForm] SpecialtyImageMultiUploadWrapperDTO dto)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "specialties");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var allImages = new List<SpecialtyImage>();

            foreach (var item in dto.Items)
            {
                // ✅ Bỏ qua nếu không có ảnh
                if (item.Files == null || item.Files.Count == 0)
                    continue;

                foreach (var file in item.Files)
                {
                    if (file.Length == 0) continue;

                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
                        continue;

                    var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                    var safeName = Regex.Replace(originalName.ToLower(), "[^a-z0-9]+", "-").Trim('-');
                    var fileName = $"{safeName}-{DateTime.UtcNow.Ticks}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    allImages.Add(new SpecialtyImage
                    {
                        SpecialtyId = item.SpecialtyId,
                        ImageUrl = $"/images/specialties/{fileName}"
                    });
                }
            }

            if (allImages.Count == 0)
                return BadRequest("Không có ảnh hợp lệ nào được tải lên.");

            _context.SpecialtyImages.AddRange(allImages);
            await _context.SaveChangesAsync();

            return Ok(allImages);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.SpecialtyImages.FindAsync(id);
            if (image == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.SpecialtyImages.Remove(image);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
