using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class ProductMvcController : Controller
    {
        // GET: /Product
        public IActionResult Index()
        {
            return View();
        }
    }
}