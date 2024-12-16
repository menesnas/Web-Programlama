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

        // Rezervasyon listeleme (Index)
        public IActionResult Index()
        {
            var rezervasyonlar = _context.Rezervasyonlar
                .Include(r => r.Personel) // Personel ile ilişkiyi sağlıyoruz
                .ToList();

            return View(rezervasyonlar); // Rezervasyonlar listesini view'a gönderiyoruz
        }

        // Yeni rezervasyon ekleme formu
        public IActionResult Ekle()
        {
            var viewModel = new RezervasyonViewModel
            {
                Rezervasyon = new Rezervasyon(), // Yeni bir rezervasyon nesnesi oluşturuyoruz
                Personeller = _context.Personeller.ToList() // Personel listesini alıyoruz
            };

            return View(viewModel); // ViewModel'i view'a gönderiyoruz
        }

        // Yeni rezervasyon ekleme işlemi (POST)
        [HttpPost]
        public IActionResult Ekle(RezervasyonViewModel viewModel)
        {
            // Personel seçimi kontrolü
            if (viewModel.SecilenPersonelId == 0)
            {
                ModelState.AddModelError("SecilenPersonelId", "Lütfen bir personel seçin.");
            }

            // Model doğrulama
            if (ModelState.IsValid)
            {
                try
                {
                    // Rezervasyon nesnesini oluşturuyoruz
                    var rezervasyon = new Rezervasyon
                    {
                        Ad = viewModel.Rezervasyon.Ad,
                        Soyad = viewModel.Rezervasyon.Soyad,
                        Tarih = viewModel.Rezervasyon.Tarih,
                        Telefon = viewModel.Rezervasyon.Telefon,
                        // Seçilen personeli alıyoruz
                        Personel = _context.Personeller
                            .FirstOrDefault(p => p.Id == viewModel.SecilenPersonelId)
                    };

                    // Veritabanına ekliyoruz
                    _context.Rezervasyonlar.Add(rezervasyon);
                    _context.SaveChanges();

                    // Başarıyla kaydedildiyse Index sayfasına yönlendiriyoruz
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Hata oluşursa hata mesajını ekliyoruz
                    ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                }
            }

            // Eğer model geçerli değilse, Personeller listesini yeniden yükleyip formu tekrar gösteriyoruz
            viewModel.Personeller = _context.Personeller.ToList();
            return View(viewModel);
        }




    }
}
