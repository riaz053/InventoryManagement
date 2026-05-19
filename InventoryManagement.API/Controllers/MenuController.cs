using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/Menu")]
    public class MenuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Menu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Menus
                .Include(m => m.ParentMenu)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.MenuName)
                .Select(m => new MenuViewDto
                {
                    MenuId = m.MenuId,
                    MenuCode = m.MenuCode,
                    MenuName = m.MenuName,
                    Link = m.Link,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder,
                    ParentMenuId = m.ParentMenuId,
                    ParentMenuName = m.ParentMenu != null
                        ? m.ParentMenu.MenuName
                        : null,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/Menu/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var menu = await _context.Menus
                .Include(m => m.ParentMenu)
                .Where(m => m.MenuId == id)
                .Select(m => new MenuViewDto
                {
                    MenuId = m.MenuId,
                    MenuCode = m.MenuCode,
                    MenuName = m.MenuName,
                    Link = m.Link,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder,
                    ParentMenuId = m.ParentMenuId,
                    ParentMenuName = m.ParentMenu != null
                        ? m.ParentMenu.MenuName
                        : null,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            if (menu == null)
                return NotFound(new { message = "Menu not found." });

            return Ok(menu);
        }

        // POST: api/Menu/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MenuCreateDto model)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(model.MenuName))
                return BadRequest(new { message = "Menu Name cannot be empty." });

            model.MenuName = model.MenuName.Trim();

            // Duplicate check
            var exists = await _context.Menus.AnyAsync(m =>
                m.MenuName == model.MenuName &&
                m.ParentMenuId == model.ParentMenuId);

            if (exists)
                return BadRequest(new
                {
                    message = "A menu with the same name already exists under this parent."
                });

            // Validate parent
            if (model.ParentMenuId.HasValue)
            {
                var parentExists = await _context.Menus.AnyAsync(m =>
                    m.MenuId == model.ParentMenuId.Value);

                if (!parentExists)
                    return BadRequest(new { message = "Selected parent menu does not exist." });
            }

            // Auto-generate MenuCode
            var lastId = await _context.Menus
                .OrderByDescending(m => m.MenuId)
                .Select(m => m.MenuId)
                .FirstOrDefaultAsync();

            var menuCode = $"MNU-{(lastId + 1):D8}";

            // Normalize Link:
            // if empty => group menu (folder)
            string? link = string.IsNullOrWhiteSpace(model.Link)
                ? null
                : model.Link.Trim();

            var menu = new Menu
            {
                MenuCode = menuCode,
                MenuName = model.MenuName,
                Link = link,
                Icon = string.IsNullOrWhiteSpace(model.Icon)
                    ? null
                    : model.Icon.Trim(),
                DisplayOrder = model.DisplayOrder,
                ParentMenuId = model.ParentMenuId,
                IsActive = true
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Menu created successfully.",
                menu.MenuId,
                menu.MenuCode
            });
        }

        // POST: api/Menu/update
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] MenuUpdateDto model)
        {
            if (string.IsNullOrWhiteSpace(model.MenuName))
                return BadRequest(new { message = "Menu Name cannot be empty." });

            var menu = await _context.Menus.FindAsync(model.MenuId);

            if (menu == null)
                return NotFound(new { message = "Menu not found." });

            model.MenuName = model.MenuName.Trim();

            // Prevent duplicate
            var exists = await _context.Menus.AnyAsync(m =>
                m.MenuId != model.MenuId &&
                m.MenuName == model.MenuName &&
                m.ParentMenuId == model.ParentMenuId);

            if (exists)
                return BadRequest(new
                {
                    message = "A menu with the same name already exists under this parent."
                });

            // Prevent self-parenting
            if (model.ParentMenuId == model.MenuId)
                return BadRequest(new
                {
                    message = "A menu cannot be its own parent."
                });

            // Validate parent
            if (model.ParentMenuId.HasValue)
            {
                var parentExists = await _context.Menus.AnyAsync(m =>
                    m.MenuId == model.ParentMenuId.Value);

                if (!parentExists)
                    return BadRequest(new { message = "Selected parent menu does not exist." });
            }

            menu.MenuName = model.MenuName;
            menu.Link = string.IsNullOrWhiteSpace(model.Link)
                ? null
                : model.Link.Trim();
            menu.Icon = string.IsNullOrWhiteSpace(model.Icon)
                ? null
                : model.Icon.Trim();
            menu.DisplayOrder = model.DisplayOrder;
            menu.ParentMenuId = model.ParentMenuId;
            menu.IsActive = model.IsActive ?? true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Menu updated successfully."
            });
        }

        // DELETE: api/Menu/5
        // Soft delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
                return NotFound(new { message = "Menu not found." });

            // Check if menu has children
            var hasChildren = await _context.Menus.AnyAsync(m =>
                m.ParentMenuId == id &&
                m.IsActive);

            if (hasChildren)
            {
                return BadRequest(new
                {
                    message = "Cannot disable this menu because it has active child menus."
                });
            }

            menu.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Menu disabled successfully."
            });
        }

        // GET: api/Menu/active
        // Used for sidebar and dropdowns
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMenus()
        {
            var data = await _context.Menus
                .Where(m => m.IsActive)
                .Include(m => m.ParentMenu)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.MenuName)
                .Select(m => new MenuViewDto
                {
                    MenuId = m.MenuId,
                    MenuCode = m.MenuCode,
                    MenuName = m.MenuName,
                    Link = m.Link,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder,
                    ParentMenuId = m.ParentMenuId,
                    ParentMenuName = m.ParentMenu != null
                        ? m.ParentMenu.MenuName
                        : null,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}