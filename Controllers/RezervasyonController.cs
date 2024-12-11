using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;  // Rezervasyon modelini kullanmak için
using Microsoft.EntityFrameworkCore;  // Include için

namespace WebApplication7.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly RezervasyonDbContext _rezervasyonContext;
        private readonly PersonelDbContext _personelContext;

        public RezervasyonController(RezervasyonDbContext rezervasyonContext, PersonelDbContext personelContext)
        {
            _rezervasyonContext = rezervasyonContext;
            _personelContext = personelContext;
        }

        public IActionResult Index()
        {
            try
            {
                // Rezervasyonlar ile Personel ilişkisini sağlıyoruz
                var rezervasyonlar = _rezervasyonContext.Rezervasyonlar
                    .Include(r => r.Personel)  // Personel bilgisini de dahil ediyoruz
                    .ToList();

                // Eğer rezervasyon listesi boşsa, mesaj gösteriyoruz
                if (rezervasyonlar.Count == 0)
                {
                    ViewBag.Message = "Rezervasyon listesi boş.";
                    return View();
                }

                // Rezervasyonlar varsa, bunları görüntülüyoruz
                return View(rezervasyonlar);
            }
            catch (Exception ex)
            {
                // Hata durumunda, mesaj gösteriyoruz
                ViewBag.Message = "Bir hata oluştu: " + ex.Message;
                return View();
            }
        }


        // Yeni rezervasyon ekleme formu
        public IActionResult Ekle()
        {
            var viewModel = new RezervasyonViewModel
            {
                Personeller = _personelContext.Personeller.ToList(), // Personel listesini alıyoruz
                Rezervasyon = new Rezervasyon() // Yeni bir rezervasyon oluşturuyoruz
            };

            return View(viewModel); // ViewModel'i view'a gönderiyoruz
        }

        [HttpPost]
        public IActionResult Ekle(RezervasyonViewModel viewModel)
        {
            // Eğer seçilen personel id'si null ise, hata ekleyelim
            if (viewModel.SecilenPersonelId == null)
            {
                ModelState.AddModelError("SecilenPersonelId", "Personel seçilmesi gerekmektedir.");
            }

            // Eğer personel id geçerli değilse (0 veya null) model hatası ekleyelim
            if (viewModel.Rezervasyon != null && (viewModel.Rezervasyon.PersonelId == 0 || viewModel.Rezervasyon.PersonelId == null))
            {
                ModelState.AddModelError("PersonelId", "Geçerli bir personel seçilmelidir.");
            }

            // Model geçerliyse, yeni rezervasyonu veritabanına ekliyoruz
            if (ModelState.IsValid)
            {
                // SecilenPersonelId'yi rezervasyon modeline atıyoruz
                viewModel.Rezervasyon.PersonelId = viewModel.SecilenPersonelId.Value;  // SecilenPersonelId null değil, Value alıyoruz

                try
                {
                    // Yeni rezervasyonu veritabanına ekliyoruz
                    _rezervasyonContext.Rezervasyonlar.Add(viewModel.Rezervasyon);
                    _rezervasyonContext.SaveChanges(); // Veritabanına kaydet

                    return RedirectToAction("Index"); // Rezervasyon listesine yönlendir
                }
                catch (Exception ex)
                {
                    // Hata oluşursa modelstate'e hata ekleyelim
                    ModelState.AddModelError("", "Veritabanına kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }

            // ModelState geçerli değilse, personel listesini tekrar yükle
            viewModel.Personeller = _personelContext.Personeller.ToList();
            return View(viewModel); // Hatalı durumda formu tekrar göster
        }

        // Rezervasyon silme işlemi
        public IActionResult Delete(int id)
        {
            var rezervasyon = _rezervasyonContext.Rezervasyonlar.Find(id); // Rezervasyonu veritabanından bul
            if (rezervasyon == null)
            {
                return NotFound(); // Rezervasyon bulunamazsa, hata mesajı döner
            }

            _rezervasyonContext.Rezervasyonlar.Remove(rezervasyon); // Rezervasyonu sil
            _rezervasyonContext.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("Index"); // Silme işleminden sonra listeye dön
        }

        // Rezervasyon düzenleme formu
        public IActionResult Edit(int id)
        {
            var rezervasyon = _rezervasyonContext.Rezervasyonlar.Find(id); // Rezervasyonu bul
            if (rezervasyon == null)
            {
                return NotFound(); // Eğer rezervasyon bulunmazsa hata döner
            }

            var viewModel = new RezervasyonViewModel
            {
                Rezervasyon = rezervasyon, // Bulunan rezervasyonu ViewModel'e aktar
                Personeller = _personelContext.Personeller.ToList() // Personel listesini alıyoruz
            };

            return View(viewModel); // Rezervasyon düzenleme formunu döner
        }

        [HttpPost]
        public IActionResult Edit(RezervasyonViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Güncellenmiş rezervasyonu veritabanına kaydet
                    _rezervasyonContext.Rezervasyonlar.Update(viewModel.Rezervasyon);
                    _rezervasyonContext.SaveChanges(); // Değişiklikleri kaydet

                    return RedirectToAction("Index"); // Güncellenmiş listeye yönlendir
                }
                catch (Exception ex)
                {
                    // Hata oluşursa modelstate'e hata ekleyelim
                    ModelState.AddModelError("", "Veritabanına kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }

            // Hatalı durum varsa formu tekrar göster
            viewModel.Personeller = _personelContext.Personeller.ToList(); // Personel listesini tekrar yükle
            return View(viewModel);
        }
    }
}
