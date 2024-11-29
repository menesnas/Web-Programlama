using Microsoft.AspNetCore.Mvc;

namespace WebApplication7.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminPanel()
        {

            return View();
        }
    }
}
