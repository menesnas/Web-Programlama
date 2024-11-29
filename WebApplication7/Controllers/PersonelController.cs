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
    }
}
