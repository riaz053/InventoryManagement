using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class RoleMenuMvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}