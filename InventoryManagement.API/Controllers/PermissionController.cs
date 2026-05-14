using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PermissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Permission
        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await _context.Permissions
                .OrderBy(p => p.PermissionId)
                .ToListAsync();

            return Ok(permissions);
        }

        // GET: api/Permission/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        // POST: api/Permission
        [HttpPost]
        public async Task<IActionResult> CreatePermission(PermissionDto dto)
        {
            var exists = await _context.Permissions
                .AnyAsync(p => p.PermissionName == dto.PermissionName);

            if (exists)
                return BadRequest("Permission already exists.");

            var permission = new Permission
            {
                PermissionName = dto.PermissionName,
                IsActive = dto.IsActive
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return Ok(permission);
        }

        // PUT: api/Permission/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, PermissionDto dto)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
                return NotFound();

            permission.PermissionName = dto.PermissionName;
            permission.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return Ok(permission);
        }

        // DELETE: api/Permission/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
                return NotFound();

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Permission deleted successfully." });
        }
    }
}