using Microsoft.AspNetCore.Mvc;

namespace WebApplication7.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminPanel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminGiris(string Ad, string Soyad, string Mail, string Sifre)
        {
            // Admin bilgilerini doğrulama işlemi
            // Örnek: Sadece sabit bir kullanıcı adı ve şifre kontrol ediliyor
            if (Mail == "a@a.com" && Sifre == "1234")
            {
                // Başarılı giriş -> Ana sayfaya yönlendirme
                return RedirectToAction("Index", "Home");
            }

            // Başarısız giriş -> Admin giriş sayfasına geri döndürme
            ViewBag.HataMesaji = "Geçersiz e-posta veya şifre.";
            return View("AdminPanel");
        }
    }
}
