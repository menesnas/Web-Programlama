using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using WebApplication7.Data;
using WebApplication7.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing; // Mutate metodu bu ad alanı altında
using SixLabors.ImageSharp.Formats.Png;


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
        public async Task<IActionResult> Ekle(RezervasyonViewModel viewModel, IFormFile ResimDosyasi, string action)
        {
            // Personel ve saç modeli listelerini doldur
            viewModel.Personeller = _context.Personeller.ToList();
            viewModel.SacModelleri = _context.SacModelleri.ToList();

            if (action == "analyze")
            {
                // Fotoğrafı analiz et butonuna basıldı.
                // images/edits endpoint'ini kullanacağız.

                // var apiKey = ""; // Kendi API anahtarınızı girin.

                if (ResimDosyasi != null && ResimDosyasi.Length > 0)
                {
                    // Resim dosyasını bellek içine yükle
                    byte[] originalImageBytes;
                    using (var ms = new MemoryStream())
                    {
                        await ResimDosyasi.CopyToAsync(ms);
                        ms.Position = 0;
                        using (Image image = Image.Load(ms)) // image değişkeni artık SixLabors.ImageSharp.Image tipinde
                        {
                            // Burada gerekirse resmi yeniden boyutlandırabilirsiniz.
                            image.Mutate(x => x.Resize(1024, 1024));

                            using (var outStream = new MemoryStream())
                            {
                                image.SaveAsPng(outStream);
                                originalImageBytes = outStream.ToArray();
                            }
                        }
                    }


                    // Mask dosyasını wwwroot dizininden oku
                    var maskPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mask.png");
                    if (!System.IO.File.Exists(maskPath))
                    {
                        ModelState.AddModelError("", "Mask dosyası bulunamadı.");
                        return View(viewModel);
                    }
                    var maskImageBytes = await System.IO.File.ReadAllBytesAsync(maskPath);

                    // Prompt
                    var prompt = "Bu fotoğraftaki kişinin yüz ve kafa şekline uygun, modern bir saç modeli ekle.";

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", apiKey);

                        var form = new MultipartFormDataContent();
                        form.Add(new ByteArrayContent(originalImageBytes), "image", "input.png");
                        form.Add(new ByteArrayContent(maskImageBytes), "mask", "mask.png");
                        form.Add(new StringContent(prompt), "prompt");
                        form.Add(new StringContent("1"), "n");
                        form.Add(new StringContent("1024x1024"), "size");

                        var response = await client.PostAsync("https://api.openai.com/v1/images/edits", form);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<ImageGenerationResponse>(jsonResponse);
                            var imageUrl = result?.Data?.FirstOrDefault()?.Url;
                            viewModel.Rezervasyon.ImageUrl = imageUrl;
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"API Hatası: {errorContent}");
                            ModelState.AddModelError("", $"Görsel düzenleme başarısız oldu: {errorContent}");
                        }

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Fotoğraf yüklemeden analiz edemezsiniz.");
                }

                // Görsel veya hata mesajıyla sayfayı geri dönüyoruz.
                return View(viewModel);
            }
            else if (action == "add")
            {
                // Rezervasyon ekleme butonuna basıldı.
                // Validasyonlar
                if (viewModel.Rezervasyon.PersonelId == 0)
                {
                    ModelState.AddModelError("Rezervasyon.PersonelId", "Lütfen bir personel seçin.");
                    return View(viewModel);
                }

                if (viewModel.Rezervasyon.SacModeliId == 0)
                {
                    ModelState.AddModelError("Rezervasyon.SacModeliId", "Lütfen bir saç modeli seçin.");
                    return View(viewModel);
                }

                // Personelin mevcut rezervasyonlarını kontrol et
                var personelRezervasyonlari = _context.Rezervasyonlar
                    .Where(r => r.PersonelId == viewModel.Rezervasyon.PersonelId)
                    .Select(r => r.Tarih)
                    .ToList();

                var yeniRezervasyonSaati = viewModel.Rezervasyon.Tarih;
                bool uygunMu = personelRezervasyonlari.All(tarih =>
                    yeniRezervasyonSaati >= tarih.AddHours(1) || yeniRezervasyonSaati < tarih);

                if (!uygunMu)
                {
                    ModelState.AddModelError("Rezervasyon.Tarih", "Seçilen personelin bu saatte rezervasyonu var. En erken 1 saat sonrası için rezervasyon yapılabilir.");
                    return View(viewModel);
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

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Default olarak sayfayı geri döndürelim
            return View(viewModel);
        }
    }

    public class ImageGenerationResponse
    {
        public ImageData[] Data { get; set; }
    }

    public class ImageData
    {
        public string Url { get; set; }
    }
}
