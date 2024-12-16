using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication7.Models; // Personel modelini kullanmak için
using WebApplication7.Data;
using System.Linq;

namespace WebApplication7.Controllers
{
    public class PersonelController : Controller
    {
        private readonly MyCustomDbContext _context;

        // Constructor ile ApplicationDbContext'i enjekte ediyoruz
        public PersonelController(MyCustomDbContext context)
        {
            _context = context; // ApplicationDbContext ile veritabanı bağlantısı sağlanıyor
        }

        // Personelleri listeleme (Index)
        public IActionResult Index()
        {
            var personeller = _context.Personeller.ToList(); // Veritabanındaki tüm personelleri al
            return View(personeller); // Personel listesini view'a gönder
        }
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Yeni personel ekleme işlemi
        [HttpPost]
        public IActionResult Create(Personel yeniPersonel)
        {
            if (ModelState.IsValid)
            {
                _context.Personeller.Add(yeniPersonel); // Yeni personeli veritabanına ekle
                _context.SaveChanges(); // Veritabanına kaydet
                return RedirectToAction("Index"); // Personel listesine yönlendir
            }
            return View(yeniPersonel); // Hatalı durum varsa formu tekrar göster
        }

        // Personel düzenleme formunu gösterme
        public IActionResult Edit(int id)
        {
            var personel = _context.Personeller.Find(id); // Veritabanından personeli bul
            if (personel == null)
            {
                return NotFound(); // Eğer personel bulunamazsa hata döner
            }
            return View(personel); // Düzenleme formunu göster
        }

        // Personel düzenleme işlemi
        [HttpPost]
        public IActionResult Edit(Personel guncellenenPersonel)
        {
            if (ModelState.IsValid)
            {
                var personel = _context.Personeller.Find(guncellenenPersonel.Id); // Veritabanından personeli bul
                if (personel == null)
                {
                    return NotFound(); // Eğer personel bulunamazsa hata döner
                }

                // Güncelleme işlemi
                personel.Ad = guncellenenPersonel.Ad;
                personel.Soyad = guncellenenPersonel.Soyad;
                personel.CalistigiSaat = guncellenenPersonel.CalistigiSaat;
                personel.GunlukKazandirdigiPara = guncellenenPersonel.GunlukKazandirdigiPara;

                _context.SaveChanges(); // Değişiklikleri kaydet
                return RedirectToAction("Index"); // Güncellenen personel listesine yönlendir
            }
            return View(guncellenenPersonel); // Hatalı durum varsa formu tekrar göster
        }

        // Personel silme işlemi
        public IActionResult Delete(int id)
        {
            var personel = _context.Personeller.Find(id); // Veritabanından personeli bul
            if (personel == null)
            {
                return NotFound(); // Eğer personel bulunamazsa hata döner
            }

            _context.Personeller.Remove(personel); // Personeli sil
            _context.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("Index"); // Personel listesini göster
        }
    }
}
