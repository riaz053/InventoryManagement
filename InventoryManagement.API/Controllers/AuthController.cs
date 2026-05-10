using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.API.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // TEMP LOGIN (we will connect DB later)
            if (username == "Admin" && password == "1234")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid login";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

// using InventoryManagement.API.Data;
// using InventoryManagement.API.DTOs;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;

// namespace InventoryManagement.API.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class AuthController : ControllerBase
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly IConfiguration _configuration;

//         public AuthController(
//             ApplicationDbContext context,
//             IConfiguration configuration
//         )
//         {
//             _context = context;
//             _configuration = configuration;
//         }
//         // [HttpPost("login")]
//         // public async Task<IActionResult> Login(LoginDto model)
//         // {
//         //     try
//         //     {
//         //         var user = await _context.Users
//         //             .Include(x => x.Role)
//         //             .FirstOrDefaultAsync(x =>
//         //                 x.Username == model.Username
//         //                 && x.PasswordHash == model.Password
//         //             );

//         //         if (user == null)
//         //         {
//         //             return Unauthorized("Invalid username or password");
//         //         }

//         //         var claims = new[]
//         //         {
//         //     new Claim(ClaimTypes.Name, user.Username),
//         //     new Claim(ClaimTypes.Role, user.Role!.RoleName)
//         // };

//         //         var key = new SymmetricSecurityKey(
//         //             Encoding.UTF8.GetBytes(
//         //                 _configuration["Jwt:Key"]!
//         //             )
//         //         );

//         //         var creds = new SigningCredentials(
//         //             key,
//         //             SecurityAlgorithms.HmacSha256
//         //         );

//         //         var token = new JwtSecurityToken(
//         //             issuer: _configuration["Jwt:Issuer"],
//         //             audience: _configuration["Jwt:Audience"],
//         //             claims: claims,
//         //             expires: DateTime.Now.AddMinutes(60),
//         //             signingCredentials: creds
//         //         );

//         //         return Ok(new
//         //         {
//         //             token = new JwtSecurityTokenHandler()
//         //                 .WriteToken(token)
//         //         });
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         return StatusCode(500, ex.Message);
//         //     }
//         // }
//         [HttpPost("login")]
//         public async Task<IActionResult> Login(LoginDto model)
//         {
//             var user = await _context.Users
//                 .Include(x => x.Role)
//                 .FirstOrDefaultAsync(x =>
//                     x.Username == model.Username
//                     && x.PasswordHash == model.Password
//                 );

//             if (user == null)
//             {
//                 return Unauthorized("Invalid username or password");
//             }

//             var claims = new[]
//             {
//                 new Claim(ClaimTypes.Name, user.Username),
//                 new Claim(
//                     ClaimTypes.Role,
//                     user.Role!.RoleName
//                 )
//             };

//             var key = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(
//                     _configuration["Jwt:Key"]!
//                 )
//             );

//             var creds = new SigningCredentials(
//                 key,
//                 SecurityAlgorithms.HmacSha256
//             );

//             var token = new JwtSecurityToken(
//                 issuer: _configuration["Jwt:Issuer"],
//                 audience: _configuration["Jwt:Audience"],
//                 claims: claims,
//                 expires: DateTime.Now.AddMinutes(60),
//                 signingCredentials: creds
//             );

//             return Ok(new
//             {
//                 token = new JwtSecurityTokenHandler()
//                     .WriteToken(token)
//             });
//         }
//     }
// }