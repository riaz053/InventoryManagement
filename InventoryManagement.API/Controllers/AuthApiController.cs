using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthApiController(
            ApplicationDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ===========================
        // LOGIN
        // ===========================
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(x =>
                    x.Username == dto.Username &&
                    x.Password == dto.Password &&
                    x.IsActive);

            if (user == null)
                return Unauthorized("Invalid Login");

            var role = _context.UserRoles
                .Where(x => x.UserId == user.Id)
                .Select(x => x.Role.RoleName)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(role))
                return Unauthorized("No role assigned");

            // NEW SESSION
            var sessionId = Guid.NewGuid().ToString();

            user.CurrentSessionId = sessionId;
            _context.SaveChanges();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, role),

                // session claim
                new Claim("sid", sessionId)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256)
            );

            var jwt = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                token = jwt,
                username = user.Username,
                role
            });
        }

        // ===========================
        // CHECK SESSION
        // ===========================
        [Authorize]
        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessionId = User.FindFirst("sid")?.Value;

            if (userId == null || sessionId == null)
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(x => x.Id.ToString() == userId);

            if (user == null)
                return Unauthorized();

            if (user.CurrentSessionId != sessionId)
                return Unauthorized();

            return Ok(new { valid = true });
        }
    }
}