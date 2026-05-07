using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL PRODUCTS
        // Admin + ProductUser
        // =========================
        [HttpGet]
        [Authorize(Roles = "Admin,ProductUser")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products.ToListAsync();

            return Ok(products);
        }

        // =========================
        // ADD PRODUCT
        // Admin + ProductUser
        // =========================
        [HttpPost]
        [Authorize(Roles = "Admin,ProductUser")]
        public async Task<IActionResult> Create(Product model)
        {
            _context.Products.Add(model);

            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // =========================
        // UPDATE PRODUCT
        // ONLY ADMIN
        // =========================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            Product model
        )
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Code = model.Code;
            product.PurchaseRate = model.PurchaseRate;
            product.SaleRate = model.SaleRate;
            product.Quantity = model.Quantity;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // =========================
        // DELETE PRODUCT
        // ONLY ADMIN
        // =========================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return Ok("Deleted Successfully");
        }
    }
}