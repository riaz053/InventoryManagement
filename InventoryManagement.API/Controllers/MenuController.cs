using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        // =========================================
        // GET ALL (ADMIN)
        // =========================================
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
                    ParentMenuName = m.ParentMenu != null ? m.ParentMenu.MenuName : null,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }

        // =========================================
        // GET BY ID
        // =========================================
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
                    ParentMenuName = m.ParentMenu != null ? m.ParentMenu.MenuName : null,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            if (menu == null)
                return NotFound();

            return Ok(menu);
        }

        // =========================================
        // CREATE
        // =========================================
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MenuCreateDto model)
        {
            if (string.IsNullOrWhiteSpace(model.MenuName))
                return BadRequest("Menu Name required");

            var exists = await _context.Menus.AnyAsync(x =>
                x.MenuName == model.MenuName &&
                x.ParentMenuId == model.ParentMenuId);

            if (exists)
                return BadRequest("Duplicate menu");

            var lastId = await _context.Menus
                .OrderByDescending(x => x.MenuId)
                .Select(x => x.MenuId)
                .FirstOrDefaultAsync();

            var menu = new Menu
            {
                MenuCode = $"MNU-{(lastId + 1):D7}",
                MenuName = model.MenuName.Trim(),
                Link = string.IsNullOrWhiteSpace(model.Link) ? null : model.Link.Trim(),
                Icon = model.Icon,
                DisplayOrder = model.DisplayOrder,
                ParentMenuId = model.ParentMenuId,
                IsActive = true
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu created", menu.MenuId });
        }

        // =========================================
        // UPDATE
        // =========================================
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] MenuUpdateDto model)
        {
            var menu = await _context.Menus.FindAsync(model.MenuId);

            if (menu == null)
                return NotFound();

            menu.MenuName = model.MenuName.Trim();
            menu.MenuCode = model.MenuCode;
            menu.Link = model.Link;
            menu.Icon = model.Icon;
            menu.DisplayOrder = model.DisplayOrder;

            if (model.ParentMenuId != model.MenuId)
                menu.ParentMenuId = model.ParentMenuId;

            if (model.IsActive.HasValue)
                menu.IsActive = model.IsActive.Value;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu updated" });
        }

        // =========================================
        // DELETE (SOFT DELETE)
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
                return NotFound();

            menu.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu disabled" });
        }

        // =========================================
        // ACTIVE MENUS
        // =========================================
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMenus()
        {
            var data = await _context.Menus
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(m => new MenuViewDto
                {
                    MenuId = m.MenuId,
                    MenuCode = m.MenuCode,
                    MenuName = m.MenuName,
                    Link = m.Link,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder,
                    ParentMenuId = m.ParentMenuId,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }

        // =========================================
        // 🔥 ROLE BASED MENU (JWT)
        // =========================================
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("by-role")]
        public async Task<IActionResult> GetMenusByRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
                return Unauthorized("Role not found in token");

            var roleId = await _context.Roles
                .Where(r => r.RoleName.Trim().ToLower() == role.Trim().ToLower())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (roleId == 0)
                return Ok("RoleId not found");

            // 1. Assigned menu IDs
            var assignedMenuIds = await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.MenuId)
                .ToListAsync();

            if (!assignedMenuIds.Any())
                return Ok("No menus assigned to this role");

            // 2. Load ALL active menus (IMPORTANT)
            var allMenus = await _context.Menus
                .Where(m => m.IsActive)
                .Select(m => new MenuTreeDto
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName,
                    Link = !string.IsNullOrWhiteSpace(m.Link)
                        ? m.Link + "/Index"
                        : null,
                    Icon = m.Icon,
                    ParentMenuId = m.ParentMenuId,
                    Children = new List<MenuTreeDto>()
                })
                .ToListAsync();

            // 3. Build required set (assigned + all parents)
            var requiredIds = new HashSet<int>(assignedMenuIds);

            foreach (var id in assignedMenuIds)
            {
                var current = allMenus.FirstOrDefault(x => x.MenuId == id);

                while (current?.ParentMenuId != null)
                {
                    requiredIds.Add(current.ParentMenuId.Value);

                    current = allMenus.FirstOrDefault(x =>
                        x.MenuId == current.ParentMenuId.Value);
                }
            }

            // 4. Filter final menus
            var filteredMenus = allMenus
                .Where(x => requiredIds.Contains(x.MenuId))
                .ToList();

            // 5. Build lookup
            var lookup = filteredMenus.ToDictionary(x => x.MenuId);

            List<MenuTreeDto> roots = new();

            foreach (var menu in filteredMenus)
            {
                if (menu.ParentMenuId != null &&
                    lookup.ContainsKey(menu.ParentMenuId.Value))
                {
                    lookup[menu.ParentMenuId.Value].Children.Add(menu);
                }
                else
                {
                    roots.Add(menu);
                }
            }

            return Ok(roots);
        }
        //[Authorize]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // [HttpGet("by-role")]
        // public async Task<IActionResult> GetMenusByRole()
        // {
        //     var role = User.FindFirst(ClaimTypes.Role)?.Value;

        //     if (string.IsNullOrEmpty(role))
        //         return Unauthorized("Role not found in token");

        //     var roleId = await _context.Roles
        //         .Where(r => r.RoleName.Trim().ToLower() == role.Trim().ToLower())
        //         .Select(r => r.Id)
        //         .FirstOrDefaultAsync();

        //     if (roleId == 0)
        //         return Ok("RoleId not found");

        //     var menuIds = await _context.RoleMenus
        //         .Where(rm => rm.RoleId == roleId)
        //         .Select(rm => rm.MenuId)
        //         .ToListAsync();

        //     if (!menuIds.Any())
        //         return Ok("No menus assigned to this role");

        //     var allMenus = await _context.Menus
        //         .Where(m => menuIds.Contains(m.MenuId) && m.IsActive)
        //         .Select(m => new MenuTreeDto
        //         {
        //             MenuId = m.MenuId,
        //             MenuName = m.MenuName,
        //             Link = !string.IsNullOrWhiteSpace(m.Link) ? m.Link + "/Index" : null,
        //             Icon = m.Icon,
        //             ParentMenuId = m.ParentMenuId,
        //             Children = new List<MenuTreeDto>()
        //         })
        //         .ToListAsync();


        //     var menus = await _context.Menus
        //         .Where(m => menuIds.Contains(m.MenuId) && m.IsActive)
        //         .Select(m => new MenuTreeDto
        //         {
        //             MenuId = m.MenuId,
        //             MenuName = m.MenuName,
        //             Link = !string.IsNullOrWhiteSpace(m.Link) ? m.Link + "/Index" : null,
        //             Icon = m.Icon,
        //             ParentMenuId = m.ParentMenuId,
        //             Children = new List<MenuTreeDto>()
        //         })
        //         .ToListAsync();

        //     // var lookup = allMenus.ToDictionary(x => x.MenuId);
        //     var lookup = menus.ToDictionary(x => x.MenuId);

        //     List<MenuTreeDto> roots = new List<MenuTreeDto>();

        //     //foreach (var menu in menus)

        //     foreach (var menu in menus)
        //     {
        //         if (menu.ParentMenuId != null && lookup.ContainsKey(menu.ParentMenuId.Value))
        //         {
        //             lookup[menu.ParentMenuId.Value].Children.Add(menu);
        //         }
        //         else
        //         {
        //             roots.Add(menu);
        //         }
        //     }

        //     return Ok(roots);

        // }


        // private void BuildTree(MenuTreeDto parent, List<MenuTreeDto> allMenus)
        // {
        //     parent.Children = allMenus
        //         .Where(x => x.ParentMenuId == parent.MenuId)
        //         .ToList();

        //     foreach (var child in parent.Children)
        //     {
        //         BuildTree(child, allMenus);
        //     }
        // }

        // =========================================
        // TREE BUILDER
        // =========================================
        private void LoadChildren(MenuTreeDto parent, List<MenuTreeDto> allMenus)
        {
            parent.Children = allMenus
                .Where(x => x.ParentMenuId == parent.MenuId)
                .OrderBy(x => x.MenuName)
                .ToList();

            foreach (var child in parent.Children)
            {
                LoadChildren(child, allMenus);
            }
        }

        [HttpGet("debug-role-full")]
        public async Task<IActionResult> DebugRole()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            var roleFromDb = await _context.Roles
                .Select(r => new { r.Id, r.RoleName })
                .ToListAsync();

            return Ok(new
            {
                jwtRole = role,
                rolesInDb = roleFromDb
            });
        }
    }
}


// using InventoryManagement.API.Data;
// using InventoryManagement.API.DTOs;
// using InventoryManagement.API.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace InventoryManagement.API.Controllers
// {
//     [ApiController]
//     [Route("api/Menu")]
//     public class MenuController : ControllerBase
//     {
//         private readonly ApplicationDbContext _context;

//         public MenuController(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         // GET: api/Menu
//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var data = await _context.Menus
//                 .Include(m => m.ParentMenu)
//                 .OrderBy(m => m.DisplayOrder)
//                 .ThenBy(m => m.MenuName)
//                 .Select(m => new MenuViewDto
//                 {
//                     MenuId = m.MenuId,
//                     MenuCode = m.MenuCode,
//                     MenuName = m.MenuName,
//                     Link = m.Link,
//                     Icon = m.Icon,
//                     DisplayOrder = m.DisplayOrder,
//                     ParentMenuId = m.ParentMenuId,
//                     ParentMenuName = m.ParentMenu != null
//                         ? m.ParentMenu.MenuName
//                         : null,
//                     IsActive = m.IsActive
//                 })
//                 .ToListAsync();

//             return Ok(data);
//         }

//         // GET: api/Menu/5
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetById(int id)
//         {
//             var menu = await _context.Menus
//                 .Include(m => m.ParentMenu)
//                 .Where(m => m.MenuId == id)
//                 .Select(m => new MenuViewDto
//                 {
//                     MenuId = m.MenuId,
//                     MenuCode = m.MenuCode,
//                     MenuName = m.MenuName,
//                     Link = m.Link,
//                     Icon = m.Icon,
//                     DisplayOrder = m.DisplayOrder,
//                     ParentMenuId = m.ParentMenuId,
//                     ParentMenuName = m.ParentMenu != null
//                         ? m.ParentMenu.MenuName
//                         : null,
//                     IsActive = m.IsActive
//                 })
//                 .FirstOrDefaultAsync();

//             if (menu == null)
//                 return NotFound(new { message = "Menu not found." });

//             return Ok(menu);
//         }

//         // POST: api/Menu/create
//         [HttpPost("create")]
//         public async Task<IActionResult> Create([FromBody] MenuCreateDto model)
//         {
//             // Validation
//             if (string.IsNullOrWhiteSpace(model.MenuName))
//                 return BadRequest(new { message = "Menu Name cannot be empty." });

//             model.MenuName = model.MenuName.Trim();

//             // Duplicate check
//             var exists = await _context.Menus.AnyAsync(m =>
//                 m.MenuName == model.MenuName &&
//                 m.ParentMenuId == model.ParentMenuId);

//             if (exists)
//                 return BadRequest(new
//                 {
//                     message = "A menu with the same name already exists under this parent."
//                 });

//             // Validate parent
//             if (model.ParentMenuId.HasValue)
//             {
//                 var parentExists = await _context.Menus.AnyAsync(m =>
//                     m.MenuId == model.ParentMenuId.Value);

//                 if (!parentExists)
//                     return BadRequest(new { message = "Selected parent menu does not exist." });
//             }

//             // Auto-generate MenuCode
//             var lastId = await _context.Menus
//                 .OrderByDescending(m => m.MenuId)
//                 .Select(m => m.MenuId)
//                 .FirstOrDefaultAsync();

//             var menuCode = $"MNU-{(lastId + 1):D8}";

//             // Normalize Link:
//             // if empty => group menu (folder)
//             string? link = string.IsNullOrWhiteSpace(model.Link)
//                 ? null
//                 : model.Link.Trim();

//             var menu = new Menu
//             {
//                 MenuCode = menuCode,
//                 MenuName = model.MenuName,
//                 Link = link,
//                 Icon = string.IsNullOrWhiteSpace(model.Icon)
//                     ? null
//                     : model.Icon.Trim(),
//                 DisplayOrder = model.DisplayOrder,
//                 ParentMenuId = model.ParentMenuId,
//                 IsActive = true
//             };

//             _context.Menus.Add(menu);
//             await _context.SaveChangesAsync();

//             return Ok(new
//             {
//                 message = "Menu created successfully.",
//                 menu.MenuId,
//                 menu.MenuCode
//             });
//         }

//         // POST: api/Menu/update
//         [HttpPost("update")]
//         public async Task<IActionResult> Update([FromBody] MenuUpdateDto model)
//         {
//             if (string.IsNullOrWhiteSpace(model.MenuName))
//                 return BadRequest(new { message = "Menu Name cannot be empty." });

//             var menu = await _context.Menus.FindAsync(model.MenuId);

//             if (menu == null)
//                 return NotFound(new { message = "Menu not found." });

//             model.MenuName = model.MenuName.Trim();

//             // Prevent duplicate
//             var exists = await _context.Menus.AnyAsync(m =>
//                 m.MenuId != model.MenuId &&
//                 m.MenuName == model.MenuName &&
//                 m.ParentMenuId == model.ParentMenuId);

//             if (exists)
//                 return BadRequest(new
//                 {
//                     message = "A menu with the same name already exists under this parent."
//                 });

//             // Prevent self-parenting
//             if (model.ParentMenuId == model.MenuId)
//                 return BadRequest(new
//                 {
//                     message = "A menu cannot be its own parent."
//                 });

//             // Validate parent
//             if (model.ParentMenuId.HasValue)
//             {
//                 var parentExists = await _context.Menus.AnyAsync(m =>
//                     m.MenuId == model.ParentMenuId.Value);

//                 if (!parentExists)
//                     return BadRequest(new { message = "Selected parent menu does not exist." });
//             }

//             menu.MenuName = model.MenuName;
//             menu.Link = string.IsNullOrWhiteSpace(model.Link)
//                 ? null
//                 : model.Link.Trim();
//             menu.Icon = string.IsNullOrWhiteSpace(model.Icon)
//                 ? null
//                 : model.Icon.Trim();
//             menu.DisplayOrder = model.DisplayOrder;
//             menu.ParentMenuId = model.ParentMenuId;
//             menu.IsActive = model.IsActive ?? true;

//             await _context.SaveChangesAsync();

//             return Ok(new
//             {
//                 message = "Menu updated successfully."
//             });
//         }

//         // DELETE: api/Menu/5
//         // Soft delete
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var menu = await _context.Menus.FindAsync(id);

//             if (menu == null)
//                 return NotFound(new { message = "Menu not found." });

//             // Check if menu has children
//             var hasChildren = await _context.Menus.AnyAsync(m =>
//                 m.ParentMenuId == id &&
//                 m.IsActive);

//             if (hasChildren)
//             {
//                 return BadRequest(new
//                 {
//                     message = "Cannot disable this menu because it has active child menus."
//                 });
//             }

//             menu.IsActive = false;

//             await _context.SaveChangesAsync();

//             return Ok(new
//             {
//                 message = "Menu disabled successfully."
//             });
//         }

//         // GET: api/Menu/active
//         // Used for sidebar and dropdowns
//         [HttpGet("active")]
//         public async Task<IActionResult> GetActiveMenus()
//         {
//             var data = await _context.Menus
//                 .Where(m => m.IsActive)
//                 .Include(m => m.ParentMenu)
//                 .OrderBy(m => m.DisplayOrder)
//                 .ThenBy(m => m.MenuName)
//                 .Select(m => new MenuViewDto
//                 {
//                     MenuId = m.MenuId,
//                     MenuCode = m.MenuCode,
//                     MenuName = m.MenuName,
//                     Link = m.Link,
//                     Icon = m.Icon,
//                     DisplayOrder = m.DisplayOrder,
//                     ParentMenuId = m.ParentMenuId,
//                     ParentMenuName = m.ParentMenu != null
//                         ? m.ParentMenu.MenuName
//                         : null,
//                     IsActive = m.IsActive
//                 })
//                 .ToListAsync();

//             return Ok(data);
//         }
//     }
// }