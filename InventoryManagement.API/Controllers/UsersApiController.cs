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
                    u.userCode,
                    u.Username,
                    u.IsActive,
                    u.Password,

                    RoleId = u.UserRoles
                        .Select(ur => ur.RoleId)
                        .FirstOrDefault(),

                    roleCode = u.UserRoles
                        .Select(ur => ur.Role.roleCode)
                        .FirstOrDefault(),

                    Role = u.UserRoles
                        .Select(ur => ur.Role.rName)
                        .FirstOrDefault()
                })
                .ToListAsync();

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

            // 🔥 STEP 3: GENERATE USER CODE
            user.userCode = $"USR-{user.Id:D9}";
            await _context.SaveChangesAsync();

            // 🔥 STEP 4: ASSIGN ROLE
            var userRole = new UserRoles
            {
                UserId = user.Id,
                RoleId = dto.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully",
                userCode = user.userCode
            });
        }



        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser(CreateUserDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // 🔥 UPDATE USER FIELDS
            user.Password = dto.Password;

            // 🔥 ROLE CHECK
            var existingRole = await _context.UserRoles
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingRole != null)
            {
                // UPDATE ROLE
                existingRole.RoleId = dto.RoleId;
            }
            else
            {
                // INSERT ROLE
                _context.UserRoles.Add(new UserRoles
                {
                    UserId = user.Id,
                    RoleId = dto.RoleId
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully",
                userCode = user.userCode
            });
        }





    }
}