using Microsoft.AspNetCore.Mvc;
using InventoryManagement.API.Data;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RolesApi
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _context.Roles.ToList();

            return Ok(roles);
        }

        // POST: api/RolesApi/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(Role role)
        {
            _context.Roles.Add(role);

            await _context.SaveChangesAsync();

            return Ok(role);
        }
    }
}