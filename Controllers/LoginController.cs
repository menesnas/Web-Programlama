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
            HttpContext.Session.SetString("UserRole", "Admin"); // Admin rolünü session'a ekle
            return RedirectToAction("AdminPanel", "Admin");
        }

        [HttpPost]
        public IActionResult UserLogin()
        {
            HttpContext.Session.SetString("UserRole", "User"); // Kullanıcı rolünü session'a ekle
            return RedirectToAction("UserPanel", "Kullanici");
        }
    }
}
