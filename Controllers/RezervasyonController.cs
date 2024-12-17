using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly MyCustomDbContext _context;

        // Constructor aracılığıyla ApplicationDbContext enjekte ediliyor
        public RezervasyonController(MyCustomDbContext context)
        {
            _context = context; // Tek bir DbContext ile çalışıyoruz
        }

        public IActionResult Index()
        {
            var _rezervasyonlar = _context.Rezervasyonlar
                                          .Include(r => r.Personel)
                                          .ToList(); // Tüm rezervasyonları liste halinde alıyoruz

            return View(_rezervasyonlar);
        }

        public IActionResult Ekle()
        {
            var viewModel = new RezervasyonViewModel
            {
                Rezervasyon = new Rezervasyon(),
                Personeller = _context.Personeller.ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Ekle(RezervasyonViewModel viewModel)
        {
            // Personeller listesini doldurmayı unutmayın
            viewModel.Personeller = _context.Personeller.ToList();

            if (viewModel.Rezervasyon.PersonelId == 0)
            {
                ModelState.AddModelError("Rezervasyon.PersonelId", "Lütfen bir personel seçin.");
            }
            else
            {
                _context.Rezervasyonlar.Add(viewModel.Rezervasyon);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }


            // ModelState hatalı ise form tekrar gösterilecek
            return View(viewModel);
        }

    }
}
