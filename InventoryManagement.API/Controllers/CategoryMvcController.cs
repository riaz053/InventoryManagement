using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class CategoryMvcController : Controller
    {
        public IActionResult Index()
        {
            //return Content("Category MVC Working");//
            Console.WriteLine("CategoryMvc Hit");
            return View();
        }
    }
}