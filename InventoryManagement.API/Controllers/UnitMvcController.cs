using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class UnitMvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}