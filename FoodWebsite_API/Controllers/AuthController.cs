using FoodWebsite_API.DTOs.Auth;
using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodWebsite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _jwtKey;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, SymmetricSecurityKey jwtKey)
        {
            _userManager = userManager;
            _configuration = configuration;
            _jwtKey = jwtKey;
        }

        // POST: /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email đã được sử dụng." });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Đăng ký thành công!" });
        }

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new {message = "Email hoặc mật khẩu không đúng."});

            var token = await GenerateJwtToken(user);
            return Ok(new { token });
        }

        // GET: /api/auth/profile (Chỉ dành cho user có token)
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized("Token is not valid or expired.");

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized("Không thể tìm thấy email từ token.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.PhoneNumber
            });
        }


        // Tạo JWT
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
