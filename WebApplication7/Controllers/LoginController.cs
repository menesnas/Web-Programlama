using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Admin girişi kontrolü
            if (model.UserType == "Admin")
            {
                if (ModelState.IsValid && model.Admin.Email == "admin@site.com" && model.Admin.Sifre == "adminpassword")
                {
                    return RedirectToAction("Index", "Home"); // Admin giriş yaptıysa Home sayfasına yönlendir
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz Admin adı veya şifre.");
                }
            }
            // Kullanıcı girişi kontrolü
            else if (model.UserType == "Kullanici")
            {
                if (ModelState.IsValid && model.Kullanici.Email == "kullanici@site.com" && model.Kullanici.Sifre == "userpassword")
                {
                    return RedirectToAction("Index", "Home"); // Kullanıcı giriş yaptıysa Home sayfasına yönlendir
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz Kullanıcı adı veya şifre.");
                }
            }

            return View("Index", model); // Hata varsa tekrar giriş sayfasını döndür
        }
    }
}
