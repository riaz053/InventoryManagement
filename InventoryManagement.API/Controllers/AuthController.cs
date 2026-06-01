
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // only frontend logout (JWT)
            return RedirectToAction("Login");
        }
    }
}
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Mvc;
// using System.Security.Claims;

// namespace InventoryManagement.API.Controllers
// {
//     public class AuthController : Controller
//     {

//         public IActionResult Login()
//         {
//             return View();
//         }

//         [HttpPost]
//         public async Task<IActionResult> Login(string username, string password)
//         {
//             // TEMP LOGIN (we will connect DB later)
//             if (username == "Admin" && password == "1234")
//             {
//                 var claims = new List<Claim>
//                 {
//                     new Claim(ClaimTypes.Name, username),
//                     new Claim(ClaimTypes.Role, "Admin")
//                 };

//                 var identity = new ClaimsIdentity(
//                     claims,
//                     CookieAuthenticationDefaults.AuthenticationScheme
//                 );

//                 var principal = new ClaimsPrincipal(identity);

//                 await HttpContext.SignInAsync(
//                     CookieAuthenticationDefaults.AuthenticationScheme,
//                     principal
//                 );

//                 return RedirectToAction("Index", "Home");
//             }

//             ViewBag.Error = "Invalid login";
//             return View();
//         }

//         public async Task<IActionResult> Logout()
//         {
//             await HttpContext.SignOutAsync();
//             return RedirectToAction("Login");
//         }
//     }
// }
