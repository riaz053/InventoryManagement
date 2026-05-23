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

        // ==============================
        // GET ROLE MENUS (TREE VIEW)
        // ==============================
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetRoleMenus(int roleId)
        {
            var menuIds = await _context.RoleMenus
                .Where(x => x.RoleId == roleId)
                .Select(x => x.MenuId)
                .ToListAsync();

            return Ok(menuIds);
        }

        // public async Task<IActionResult> GetRoleMenus(int roleId)
        // {
        //     var assignedMenus = await _context.RoleMenus
        //         .Where(x => x.RoleId == roleId)
        //         .Select(x => x.MenuId)
        //         .ToListAsync();

        //     var menus = await _context.Menus
        //         .Where(x => x.IsActive)
        //         .ToListAsync();

        //     var result = menus
        //         .Where(x => x.ParentMenuId == null)
        //         .Select(parent => new
        //         {
        //             parent.MenuId,
        //             parent.MenuName,

        //             children = menus
        //                 .Where(c => c.ParentMenuId == parent.MenuId)
        //                 .Select(c => new
        //                 {
        //                     c.MenuId,
        //                     c.MenuName,
        //                     isChecked = assignedMenus.Contains(c.MenuId)
        //                 })
        //         });

        //     return Ok(result);
        // }

        // ==============================
        // TOGGLE ROLE MENU (CHECKBOX)
        // ==============================
        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle(RoleMenuDto dto)
        {
            var exists = await _context.RoleMenus
                .FirstOrDefaultAsync(x =>
                    x.RoleId == dto.RoleId &&
                    x.MenuId == dto.MenuId);

            if (exists == null)
            {
                _context.RoleMenus.Add(new RoleMenu
                {
                    RoleId = dto.RoleId,
                    MenuId = dto.MenuId
                });
            }
            else
            {
                _context.RoleMenus.Remove(exists);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Updated successfully"
            });
        }
    }
}