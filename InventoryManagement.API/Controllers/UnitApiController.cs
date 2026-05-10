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
            return Ok(await _context.Units.ToListAsync());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Units model)
        {
            _context.Units.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Unit saved successfully" });
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Units model)
        {
            var data = await _context.Units.FindAsync(model.Id);

            if (data == null)
                return NotFound();

            data.unitName = model.unitName;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Unit updated successfully" });
        }
    }
}