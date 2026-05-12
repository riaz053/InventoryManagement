using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.PurchasePrice,
                    p.SalesPrice,
                    p.ReorderLevel,


                    p.CategoryId,
                    CatCode = p.Category.catCode,
                    CategoryName = p.Category.catName,   // ✅ ADD THIS

                    p.UnitId,
                    UnitCode = p.Unit.unitCode,
                    UnitName = p.Unit.unitName,

                    // p.CategoryId,
                    // CatCode = p.Category.catCode,

                    // p.UnitId,
                    // UnitCode = p.Unit.unitCode,

                    p.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }
        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _context.Products
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.ProductCode,
                    x.ProductName,
                    x.PurchasePrice,
                    x.SalesPrice,
                    x.ReorderLevel,

                    x.CategoryId,
                    CatCode = x.Category.catCode,
                    CategoryName = x.Category.catName,   // ✅ ADD THIS

                    x.UnitId,
                    UnitCode = x.Unit.unitCode,
                    UnitName = x.Unit.unitName,

                    x.IsActive
                })
                .FirstOrDefaultAsync();

            return Ok(p);
        }



        // =========================
        // CREATE
        // =========================
        [HttpPost("create")]
        public async Task<IActionResult> Create(ProductDto model)
        {
            var product = new Product
            {
                ProductName = model.ProductName,
                CategoryId = model.CategoryId,
                UnitId = model.UnitId,
                PurchasePrice = model.PurchasePrice,
                SalesPrice = model.SalesPrice,
                ReorderLevel = model.ReorderLevel,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            // generate code
            var lastId = await _context.Products
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            product.ProductCode = $"PRD-{(lastId + 1):D8}";

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product created successfully" });
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPost("update")]
        public async Task<IActionResult> Update(ProductUpdateDto model)
        {
            var data = await _context.Products.FindAsync(model.Id);

            if (data == null)
                return NotFound(new { message = "Product not found" });

            // Duplicate check (safe + correct)
            bool duplicate = await _context.Products.AnyAsync(p =>
                p.Id != model.Id &&
                p.ProductName == model.ProductName &&
                p.UnitId == model.UnitId);

            if (duplicate)
                return BadRequest(new { message = "This product already exists for selected unit" });

            // ONLY editable fields
            data.ProductName = model.ProductName;
            data.PurchasePrice = model.PurchasePrice;
            data.SalesPrice = model.SalesPrice;
            data.ReorderLevel = model.ReorderLevel;
            data.CategoryId = model.CategoryId;
            data.UnitId = model.UnitId;
            data.IsActive = model.IsActive;

            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully" });
        }
        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.Products.FindAsync(id);

            if (data == null)
                return NotFound(new { message = "Product not found" });

            _context.Products.Remove(data);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }

    }


}