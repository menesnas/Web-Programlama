using Microsoft.AspNetCore.Mvc;

namespace WebApplication7.Controllers
{
    public class KullaniciController : Controller
    {

    public IActionResult UserPanel()
    {
        return View(); // Bu View, "Views/Kullanici/UserPanel.cshtml" yolunda olmalı
    }
        // Kullanıcı giriş işlemi (Giriş Yap butonu)
        [HttpPost]
        public IActionResult KullaniciGiris(string action, string Mail, string Sifre)
        {
            if (action == "Giris")
            {
                // Giriş yap butonuna basıldığında ana sayfaya yönlendirme
                return RedirectToAction("Index", "Home");
            }
            else if (action == "Kayit")
            {
                // Kayıt ol butonuna basıldığında kayıt sayfasına yönlendirme
                return RedirectToAction("KayitOl");
            }

            return View("UserPanel"); // Varsayılan olarak aynı sayfaya dön
        }


        // Kayıt ol işlemi (Kayıt Ol butonu)
        [HttpPost]
        public IActionResult KullaniciKayit()
        {
            // Kullanıcı kayıt işlemleri
            return RedirectToAction("KayitOl", "Kullanici"); // Kayıt ol sayfasına yönlendir
        }

        public IActionResult KayitOl()
        {
            return View("Index", "Home"); // Kayıt ol sayfasını döndür
        }
    }
}
