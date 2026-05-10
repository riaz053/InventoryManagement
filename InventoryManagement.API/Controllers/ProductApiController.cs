using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Data;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductApi
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Select(p => new
                {
                    p.Id,
                    p.pName,
                    p.Price,
                    p.IsActive,

                    CategoryId = p.CategoryId,
                    catName = p.Category.catName,

                    UnitId = p.UnitId,
                    unitName = p.Unit.unitName
                })
                .ToListAsync();

            return Ok(data);
        }

        // POST: api/ProductApi/create
        [HttpPost("create")]
        public async Task<IActionResult> Create(Product model)
        {
            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product created successfully" });
        }

        // POST: api/ProductApi/update
        [HttpPost("update")]
        public async Task<IActionResult> Update(Product model)
        {
            var data = await _context.Products.FindAsync(model.Id);

            if (data == null)
                return NotFound();

            data.pName = model.pName;
            data.CategoryId = model.CategoryId;
            data.UnitId = model.UnitId;
            data.Price = model.Price;
            data.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully" });
        }
    }
}