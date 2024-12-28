using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using WebApplication7.Data;
using WebApplication7.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace WebApplication7.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly MyCustomDbContext _context;

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
                SacModelleri = _context.SacModelleri.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(RezervasyonViewModel viewModel, string action)
        {
            viewModel.Personeller = _context.Personeller.ToList();
            viewModel.SacModelleri = _context.SacModelleri.ToList();

            if (action == "generate")
            {
                // Resim dosyasının yüklendiğini kontrol et
                if (Request.Form.Files["ResimDosyasi"] == null || Request.Form.Files["ResimDosyasi"].Length == 0)
                {
                    ModelState.AddModelError("ResimDosyasi", "Lütfen bir resim dosyası yükleyin.");
                    return View(viewModel); // Form tekrar gösterilir
                }
                try
                {
                    var apiKey = ""; // Burada kendi API anahtarınızı kullanın.
                    var prompt = "Generate three separate photorealistic images of the same face, each featuring a different modern hairstyle (e.g., a fade, a slick back, and a quiff). Maintain the same facial features and lighting in each image, and only change the hair style.";

                    Console.WriteLine("DALL-E API çağrısı başlatılıyor...");
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                        var content = new StringContent(JsonSerializer.Serialize(new
                        {
                            prompt = prompt,
                            n = 1,
                            size = "1024x1024"
                        }), System.Text.Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);

                        Console.WriteLine($"API isteği tamamlandı. Status Code: {response.StatusCode}");

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<ImageGenerationResponse>(jsonResponse);

                            if (result != null && result.Data != null && result.Data.Count > 0)
                            {
                                var imageUrl = result.Data.FirstOrDefault()?.Url;
                                Console.WriteLine($"Görsel URL'si: {imageUrl}");
                                viewModel.Rezervasyon.ImageUrl = imageUrl;
                            }
                            else
                            {
                                Console.WriteLine("JSON deserialization sonucu boş döndü.");
                                ModelState.AddModelError("", "API'den görsel URL alınamadı.");
                            }
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"API Hatası: {errorContent}");
                            ModelState.AddModelError("", $"Görsel düzenleme başarısız oldu: {errorContent}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                    ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                }

                return View(viewModel);
            }
            else if (action == "add")
            {
                // Personelin mevcut rezervasyonlarını kontrol et
                var personelRezervasyonlari = _context.Rezervasyonlar
                    .Where(r => r.PersonelId == viewModel.Rezervasyon.PersonelId)
                    .Select(r => r.Tarih)
                    .ToList();

                var yeniRezervasyonSaati = viewModel.Rezervasyon.Tarih;
                bool uygunMu = personelRezervasyonlari.All(tarih =>
                    yeniRezervasyonSaati >= tarih.AddHours(1) || yeniRezervasyonSaati < tarih);

                // Rezervasyon ekleme butonuna basıldı.
                // Validasyonlar
                if (viewModel.Rezervasyon.PersonelId == 0)
                {
                    ModelState.AddModelError("Rezervasyon.PersonelId", "Lütfen bir personel seçin.");
                    return View(viewModel);
                }
                else if (viewModel.Rezervasyon.SacModeliId == 0)
                {
                    ModelState.AddModelError("Rezervasyon.SacModeliId", "Lütfen bir saç modeli seçin.");
                    return View(viewModel);
                }
                if (!uygunMu)
                {
                    ModelState.AddModelError("Rezervasyon.Tarih", "Seçilen personelin bu saatte rezervasyonu var. En erken 1 saat sonrası için rezervasyon yapılabilir.");
                    return View(viewModel);
                }
                else
                {
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

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

            }
            // Varsayılan durum
            ModelState.AddModelError("", "Geçersiz işlem.");
            return View(viewModel);
        }
    }

    public class ImageGenerationResponse
    {
        [JsonPropertyName("data")]
        public List<ImageData> Data { get; set; }
    }

    public class ImageData
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}