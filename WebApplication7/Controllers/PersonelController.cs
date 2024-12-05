using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;  // Personel modelini kullanmak için
using System.Linq;

namespace WebApplication7.Controllers
{
    public class PersonelController : Controller
    {
        private readonly PersonelDbContext _context;

        // Constructor ile ApplicationDbContext'i enjekte ediyoruz
        public PersonelController(PersonelDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var personeller = _context.Personeller.ToList(); // Veritabanındaki personelleri al
            return View(personeller); // Personel listesini view'a gönder
        }

        public IActionResult Create()
        {
            return View(); // Create.cshtml sayfasını döner
        }

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

        // Düzenleme formunu gösteren action
        public IActionResult Edit(int id)
        {
            var personel = _context.Personeller.Find(id); // Veritabanından personeli bul
            if (personel == null)
            {
                return NotFound(); // Eğer personel bulunamazsa hata döner
            }
            return View(personel); // Düzenleme formunu göster
        }

        // Düzenleme işlemini işleyen action
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

        // Silme işlemini gerçekleştiren action
        public IActionResult Delete(int id)
        {
            var personel = _context.Personeller.Find(id); // Veritabanından personeli bul
            if (personel == null)
            {
                return NotFound(); // Eğer personel bulunamazsa hata döner
            }

            _context.Personeller.Remove(personel); // Personeli sil
            _context.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("Index"); // Listeye geri dön
        }
    }
}
