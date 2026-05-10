using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class CategoryMvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}