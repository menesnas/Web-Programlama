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
                try
                {
                    var apiKey = ""; // Burada kendi API anahtarınızı kullanın.
                    var prompt = "Add a modern hairstyle to the same face by changing the hair of the face in the given image.";

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
                // Rezervasyon ekleme işlemleri
                Console.WriteLine("Rezervasyon ekleme işlemi başlatıldı...");
                // Burada gerekli kodlar yer alacak.
            }

            return View(viewModel);
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
}
