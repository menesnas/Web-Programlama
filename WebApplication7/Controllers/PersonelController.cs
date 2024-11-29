using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using System.Collections.Generic;

namespace WebApplication7.Controllers
{
    public class PersonelController : Controller
    {
        // Veritabanı veya başka bir veri kaynağından personel verileri alabilirsiniz
        private static List<Personel> personeller = new List<Personel>
        {
            new Personel { Ad = "Ahmet", Soyad = "Yılmaz", CalistigiSaat = "8", GunlukKazandirdigiPara = "150" },
            new Personel { Ad = "Mehmet", Soyad = "Kara", CalistigiSaat = "7", GunlukKazandirdigiPara = "130" }
        };

        public IActionResult Index()
        {
            return View(personeller); // Personel listesini view'a gönderiyoruz
        }

        // Yeni personel ekleme formunu gösteren action
        public IActionResult Create()
        {
            return View(); // Create.cshtml sayfasını döner
        }

        // Formdan gelen bilgileri işleyen action (HTTP POST)
        [HttpPost]
        public IActionResult Create(Personel yeniPersonel)
        {
            if (ModelState.IsValid)
            {
                personeller.Add(yeniPersonel); // Yeni personeli listeye ekle
                return RedirectToAction("Index"); // Personel listesine yönlendir
            }
            return View(yeniPersonel); // Hatalı durum varsa formu tekrar göster
        }
    }
}
