using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class MenuMvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}