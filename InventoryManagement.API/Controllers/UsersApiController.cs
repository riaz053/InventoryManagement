using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UsersApi
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.IsActive,

                        RoleId = u.UserRoles
                            .Select(r => r.RoleId)
                            .FirstOrDefault(),

                        Role = u.UserRoles
                            .Select(r => r.Role.rName)
                            .FirstOrDefault()
                    }).ToListAsync();

            return Ok(users);
        }

        // POST: api/UsersApi/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            // 🔥 STEP 1: CHECK DUPLICATE USERNAME
            var existingUser = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username.Trim().ToLower() == dto.Username.Trim().ToLower()
            );

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Username already exists"
                });
            }

            // 🔥 STEP 2: CREATE USER
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 🔥 STEP 3: ASSIGN ROLE
            var userRole = new UserRoles
            {
                UserId = user.Id,
                RoleId = dto.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully"
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser(CreateUserDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Id);

            if (user == null)
            {
                return NotFound();
            }

            // update password only
            user.Password = dto.Password;

            // find existing role mapping
            var existingRole = await _context.UserRoles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingRole != null)
            {
                // UPDATE existing role
                existingRole.RoleId = dto.RoleId;
            }
            else
            {
                // INSERT new role mapping
                var userRoles = new UserRoles
                {
                    UserId = user.Id,
                    RoleId = dto.RoleId
                };

                _context.UserRoles.Add(userRoles);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully"
            });
        }





    }
}