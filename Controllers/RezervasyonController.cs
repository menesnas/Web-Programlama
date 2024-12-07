using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;

public class RezervasyonController : Controller
{
    private static List<Rezervasyon> _rezervasyonlar = new List<Rezervasyon>();

    public IActionResult Index()
    {
        return View(_rezervasyonlar);
    }

    public IActionResult Ekle()
    {
        var viewModel = new RezervasyonViewModel
        {
            Rezervasyon = new Rezervasyon()
            // SacModelleri yüklemeye gerek yok
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Ekle(RezervasyonViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            _rezervasyonlar.Add(viewModel.Rezervasyon); // Rezervasyonu kaydediyoruz
            return RedirectToAction("Index");
        }

        return View(viewModel); // Hatalı durum için
    }
}
