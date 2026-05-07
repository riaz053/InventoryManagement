using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class UsersMvcController : Controller
    {
        // GET: /UsersMvc
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}