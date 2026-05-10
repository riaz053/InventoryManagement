using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Data;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // GET: api/CategoryApi
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _context.Categories
                .OrderBy(x => x.catCode)
                .Select(c => new
                {
                    c.Id,
                    c.catCode,
                    c.catName
                })
                .ToListAsync();

            return Ok(data);
        }

        // ==========================
        // CREATE: api/CategoryApi/create
        // ==========================
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Category model)
        {
            if (string.IsNullOrWhiteSpace(model.catName))
            {
                return BadRequest(new { message = "Category name is required" });
            }

            // 1. Save first to generate Id
            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            // 2. Generate code
            model.catCode = $"CAT-{model.Id:D7}";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category saved successfully",
                catCode = model.catCode,
                id = model.Id
            });
        }

        // ==========================
        // UPDATE: api/CategoryApi/update
        // ==========================
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Category model)
        {
            var data = await _context.Categories.FindAsync(model.Id);

            if (data == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            if (string.IsNullOrWhiteSpace(model.catName))
            {
                return BadRequest(new { message = "Category name is required" });
            }

            // ONLY editable field
            data.catName = model.catName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category updated successfully",
                catCode = data.catCode,
                id = data.Id
            });
        }
    }
}