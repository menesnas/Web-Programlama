using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using System.Collections.Generic;

public class RezervasyonController : Controller
{
    // Bellekte tutulacak rezervasyonlar listesi
    private static List<Rezervasyon> rezervasyonlar = new List<Rezervasyon>();

    // Rezervasyonları listelemek için
    public IActionResult Index()
    {
        return View(rezervasyonlar); // Bellekteki rezervasyonları gösteriyoruz
    }

    // Rezervasyon eklemek için
    public IActionResult Ekle()
    {
        var viewModel = new RezervasyonViewModel
        {
            Rezervasyon = new Rezervasyon(),
            // Personel listesini sabit olarak tanımlıyoruz (örnek)
            Personeller = new List<Personel>
            {
                new Personel { Id = 1, Ad = "Kürşat Aras" },
                new Personel { Id = 2, Ad = "Mehmet Yılmaz" },
                new Personel { Id = 3, Ad = "Ayşe Kaya" }
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Ekle(RezervasyonViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            // Bellekteki rezervasyonlar listesine yeni rezervasyonu ekliyoruz
            rezervasyonlar.Add(viewModel.Rezervasyon);
            return RedirectToAction("Index");
        }

        // Hatalı durumda Personel listesini tekrar yükleyip formu geri gönderiyoruz
        viewModel.Personeller = new List<Personel>
        {
            new Personel { Id = 1, Ad = "Kürşat Aras" },
            new Personel { Id = 2, Ad = "Mehmet Yılmaz" },
            new Personel { Id = 3, Ad = "Ayşe Kaya" }
        };
        return View(viewModel);
    }
}
