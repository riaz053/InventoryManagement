using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

// using Microsoft.AspNetCore.Mvc;

// namespace InventoryManagement.API.Controllers
// {
//     public class HomeController : Controller
//     {
//         public IActionResult Index()
//         {
//             return View();
//         }
//     }
// }
