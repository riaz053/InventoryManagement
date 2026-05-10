using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using InventoryManagement.API.DTOs;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/RolesApi
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .OrderBy(r => r.roleCode)
                .Select(r => new
                {
                    r.Id,
                    r.roleCode,
                    r.rName
                })
                .ToListAsync();

            return Ok(roles);
        }



        // POST: /api/RolesApi/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(RoleDto dto)
        {
            // Check duplicate role name
            var exists = await _context.Roles
                .AnyAsync(r => r.rName == dto.rName);

            if (exists)
            {
                return BadRequest(new { message = "Role already exists" });
            }

            // Create role
            var role = new Role
            {
                rName = dto.rName
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Generate role code after Id is created
            role.roleCode = $"ROLE-{role.Id:D7}";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role created successfully",
                roleCode = role.roleCode
            });
        }



        // POST: /api/RolesApi/update
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole(RoleDto dto)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            // Check duplicate role name (excluding current record)
            var duplicate = await _context.Roles
                .AnyAsync(r =>
                    r.Id != dto.Id &&
                    r.rName == dto.rName);

            if (duplicate)
            {
                return BadRequest(new { message = "Role already exists" });
            }

            // Update only role name
            // roleCode remains unchanged
            role.rName = dto.rName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role updated successfully",
                roleCode = role.roleCode
            });
        }


    }
}