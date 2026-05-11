using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Data;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UnitApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _context.Units
                .OrderBy(x => x.unitCode)
                .Select(c => new
                {
                    c.Id,
                    c.unitCode,
                    c.unitName
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Units model)
        {
            if (string.IsNullOrWhiteSpace(model.unitName))
            {
                return BadRequest(new { message = "Unit name is required" });
            }

            // 1. Save first to generate Id
            _context.Units.Add(model);
            await _context.SaveChangesAsync();

            // 2. Generate code
            model.unitCode = $"UN-{model.Id:D7}";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Unit saved successfully",
                unitCode = model.unitCode,
                id = model.Id
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Units model)
        {
            var data = await _context.Units.FindAsync(model.Id);

            if (data == null)
            {
                return NotFound(new { message = "Unit not found" });
            }

            if (string.IsNullOrWhiteSpace(model.unitName))
            {
                return BadRequest(new { message = "Unit name is required" });
            }

            // ONLY editable field
            data.unitName = model.unitName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Unit updated successfully",
                unitCode = data.unitCode,
                id = data.Id
            });
        }


        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Units model)
        {
            var data = await _context.Units.FindAsync(model.Id);

            if (data == null)
            {
                return NotFound(new { message = "Unit not found" });
            }

            _context.Units.Remove(data);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Unit deleted successfully",
                unitCode = data.unitCode,
                id = data.Id
            });
        }
    }
}