using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class PermissionMvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}