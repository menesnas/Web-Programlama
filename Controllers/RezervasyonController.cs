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

        public IActionResult Index(DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var query = _context.Rezervasyonlar
                                .Include(r => r.Personel)
                                .Include(r => r.Sacmodel)
                                .AsQueryable();

            // Tarih filtresi uygulanıyor
            if (baslangicTarihi.HasValue)
            {
                query = query.Where(r => r.Tarih >= baslangicTarihi.Value);
            }
            if (bitisTarihi.HasValue)
            {
                query = query.Where(r => r.Tarih <= bitisTarihi.Value);
            }

            var _rezervasyonlar = query.ToList();

            return View(_rezervasyonlar);
        }
    

        public IActionResult Ekle()
        {
            var viewModel = new RezervasyonViewModel
            {
                Rezervasyon = new Rezervasyon(),
                Personeller = _context.Personeller.ToList(),
                SacModelleri = _context.SacModelleri.ToList() // Saç modellerini listeye ekle

            };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Ekle(RezervasyonViewModel viewModel)
        {
            // Personel ve saç modeli listelerini doldur
            viewModel.Personeller = _context.Personeller.ToList();
            viewModel.SacModelleri = _context.SacModelleri.ToList();

            // Validasyon: Personel seçilmemişse hata ekle
            if (viewModel.Rezervasyon.PersonelId == 0)
            {
                ModelState.AddModelError("Rezervasyon.PersonelId", "Lütfen bir personel seçin.");
                return View(viewModel); // Sadece ilgili hata gösterilecek
            }

            // Validasyon: Saç modeli seçilmemişse hata ekle
            if (viewModel.Rezervasyon.SacModeliId == 0)
            {
                ModelState.AddModelError("Rezervasyon.SacModeliId", "Lütfen bir saç modeli seçin.");
                return View(viewModel); // Sadece ilgili hata gösterilecek
            }

            // Personelin mevcut rezervasyonlarını kontrol et
            var personelRezervasyonlari = _context.Rezervasyonlar
                .Where(r => r.PersonelId == viewModel.Rezervasyon.PersonelId)
                .Select(r => r.Tarih)
                .ToList();

            var yeniRezervasyonSaati = viewModel.Rezervasyon.Tarih;

            // Saat uygunluk kontrolü
            bool uygunMu = personelRezervasyonlari.All(tarih =>
                yeniRezervasyonSaati >= tarih.AddHours(1) || yeniRezervasyonSaati < tarih);

            if (!uygunMu)
            {
                ModelState.AddModelError("Rezervasyon.Tarih", "Seçilen personelin bu saatte rezervasyonu var. En erken 1 saat sonrası için rezervasyon yapılabilir.");
                return View(viewModel); // Sadece ilgili hata gösterilecek
            }

            // Rezervasyon ekleme işlemi
            _context.Rezervasyonlar.Add(viewModel.Rezervasyon);

            // Personelin çalışma saati ve ücret güncellemesi
            var personel = _context.Personeller.FirstOrDefault(p => p.Id == viewModel.Rezervasyon.PersonelId);
            var sacModeli = _context.SacModelleri.FirstOrDefault(s => s.Id == viewModel.Rezervasyon.SacModeliId);

            if (personel != null && sacModeli != null)
            {
                personel.CalistigiSaat += 1;
                personel.GunlukKazandirdigiPara += sacModeli.Ucret;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
