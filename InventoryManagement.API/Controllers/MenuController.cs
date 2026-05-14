using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Menus
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new MenuViewDto
                {
                    MenuId = x.MenuId,
                    MenuName = x.MenuName,
                    MenuCode = x.MenuCode,
                    Link = x.Link,
                    Icon = x.Icon,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive,
                    IsGroup = x.IsGroup,
                    ParentMenuId = x.ParentMenuId,
                    ParentMenuName = x.ParentMenu != null
                        ? x.ParentMenu.MenuName
                        : null
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var menu = await _context.Menus
                .Where(x => x.MenuId == id)
                .Select(x => new MenuViewDto
                {
                    MenuId = x.MenuId,
                    MenuName = x.MenuName,
                    MenuCode = x.MenuCode,
                    Link = x.Link,
                    Icon = x.Icon,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive,
                    IsGroup = x.IsGroup,
                    ParentMenuId = x.ParentMenuId,
                    ParentMenuName = x.ParentMenu != null
                        ? x.ParentMenu.MenuName
                        : null
                })
                .FirstOrDefaultAsync();

            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            return Ok(menu);
        }

        // CREATE
        [HttpPost("create")]
        public async Task<IActionResult> Create(MenuCreateDto model)
        {
            var exists = await _context.Menus
                .AnyAsync(x => x.MenuName == model.MenuName);

            if (exists)
                return BadRequest(new { message = "Menu already exists" });

            var menu = new Menu
            {
                MenuName = model.MenuName,
                MenuCode = model.MenuCode,
                Link = model.Link,
                Icon = model.Icon,
                DisplayOrder = model.DisplayOrder,
                ParentMenuId = model.ParentMenuId,
                IsGroup = model.IsGroup,
                IsActive = model.IsActive
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Menu created successfully",
                menu.MenuId
            });
        }

        // UPDATE
        [HttpPost("update")]
        public async Task<IActionResult> Update(MenuUpdateDto model)
        {
            var menu = await _context.Menus.FindAsync(model.MenuId);

            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            menu.MenuName = model.MenuName;
            menu.MenuCode = model.MenuCode;
            menu.Link = model.Link;
            menu.Icon = model.Icon;
            menu.DisplayOrder = model.DisplayOrder;
            menu.ParentMenuId = model.ParentMenuId;
            menu.IsGroup = model.IsGroup;
            menu.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu updated successfully" });
        }

        // DELETE (SOFT)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            menu.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu disabled successfully" });
        }
    }
}