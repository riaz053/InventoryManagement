using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleMenuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoleMenuApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================
        // GET ROLE MENUS
        // =====================================
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleMenus(int roleId)
        {
            var assignedMenus = await _context.RoleMenus
                .Where(x => x.RoleId == roleId && x.IsAllowed)
                .Select(x => x.MenuId)
                .ToListAsync();

            var menus = await _context.Menus
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new
                {
                    x.MenuId,
                    x.MenuName,
                    x.ParentMenuId,

                    isAllowed =
                        assignedMenus.Contains(x.MenuId)
                })
                .ToListAsync();

            return Ok(menus);
        }

        // =====================================
        // SAVE ROLE MENUS
        // =====================================
        [HttpPost("save")]
        public async Task<IActionResult> Save(RoleMenuSaveDto dto)
        {
            var oldMenus = await _context.RoleMenus
                .Where(x => x.RoleId == dto.RoleId)
                .ToListAsync();

            _context.RoleMenus.RemoveRange(oldMenus);

            foreach (var item in dto.Menus)
            {
                if (item.IsAllowed)
                {
                    _context.RoleMenus.Add(new RoleMenu
                    {
                        RoleId = dto.RoleId,
                        MenuId = item.MenuId,
                        IsAllowed = true
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Menus assigned successfully"
            });
        }
    }
}