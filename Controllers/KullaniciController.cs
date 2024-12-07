using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using System.Linq;

namespace WebApplication7.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly KullaniciDbContext _context;

        public KullaniciController(KullaniciDbContext context)
        {
            _context = context;
        }

        public IActionResult UserPanel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KullaniciGiris(string action, string Mail, string Sifre)
        {
            if (action == "Giris")
            {
                if (string.IsNullOrEmpty(Mail) || string.IsNullOrEmpty(Sifre))
                {
                    ViewBag.ErrorMessage = "Lütfen tüm alanları doldurun.";
                    return View("UserPanel");
                }

                // Kullanıcıyı veritabanında kontrol et
                var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Mail == Mail && k.Sifre == Sifre);
                if (kullanici != null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ErrorMessage = "Hatalı e-posta veya şifre.";
                return View("UserPanel");
            }
            else if (action == "Kayit")
            {
                return RedirectToAction("KayitOl");
            }

            return View("UserPanel");
        }

        // Kullanıcı Kayıt İşlemi
        [HttpPost]
        public IActionResult KullaniciKayit(Kullanici yeniKullanici)
        {
            if (ModelState.IsValid)
            {
                // E-posta zaten var mı kontrol et
                var kullaniciVarMi = _context.Kullanicilar.Any(k => k.Mail == yeniKullanici.Mail);
                if (kullaniciVarMi)
                {
                    ViewBag.ErrorMessage = "Bu e-posta adresi zaten kayıtlı.";
                    return View("KayitOl");
                }

                // Yeni kullanıcıyı veritabanına ekleyin
                _context.Kullanicilar.Add(yeniKullanici);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            // Model geçerli değilse, hata mesajlarını göster ve tekrar kayıt formuna dön
            return View("KayitOl");
        }

        // Kayıt Ol Sayfası
        public IActionResult KayitOl()
        {
            return View();
        }
    }
}
