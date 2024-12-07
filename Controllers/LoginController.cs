using Microsoft.AspNetCore.Mvc;

namespace WebApplication7.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin()
        {
            return RedirectToAction("AdminPanel", "Admin");
        }

        [HttpPost]
        public IActionResult UserLogin()
        {
            return RedirectToAction("UserPanel", "Kullanici");
        }
    }
}
