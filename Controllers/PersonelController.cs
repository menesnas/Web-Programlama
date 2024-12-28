using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;
using WebApplication7.Data;
using System.Linq;

namespace WebApplication7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonelController : Controller
    {
        private readonly MyCustomDbContext _context;

        public PersonelController(MyCustomDbContext context)
        {
            _context = context;
        }

        // Personel listesi (Index)
        [HttpGet]
        public IActionResult Index()
        {
            var personeller = _context.Personeller.ToList();
            return View("../Personel/Index", personeller);
        }

        // Yeni personel ekleme formu (Create GET)
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("../Personel/Create");
        }

        // Yeni personel ekleme işlemi (Create POST)
        [HttpPost("Create")]
        [ValidateAntiForgeryToken] // CSRF koruması için
        public IActionResult Create([FromForm] Personel yeniPersonel)
        {
            if (ModelState.IsValid)
            {
                _context.Personeller.Add(yeniPersonel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("../Personel/Create", yeniPersonel);
        }

        // Personel düzenleme formu (Edit GET)
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var personel = _context.Personeller.Find(id);
            if (personel == null)
            {
                return NotFound();
            }
            return View("../Personel/Edit", personel);
        }

        // Personel düzenleme işlemi (Edit POST)
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken] // CSRF koruması için
        public IActionResult Edit([FromForm] Personel guncellenenPersonel)
        {
            if (ModelState.IsValid)
            {
                var personel = _context.Personeller.Find(guncellenenPersonel.Id);
                if (personel == null)
                {
                    return NotFound();
                }

                personel.Ad = guncellenenPersonel.Ad;
                personel.Soyad = guncellenenPersonel.Soyad;
                personel.CalistigiSaat = guncellenenPersonel.CalistigiSaat;
                personel.GunlukKazandirdigiPara = guncellenenPersonel.GunlukKazandirdigiPara;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("../Personel/Edit", guncellenenPersonel);
        }

        // GET: Silme onay sayfası
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var personel = _context.Personeller.Find(id);
            if (personel == null)
                return NotFound();

            // Personeli Delete.cshtml sayfasına gönderiyoruz (onay için).
            return View("../Personel/Delete", personel);
        }

        // POST: Gerçek silme işlemi
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var personel = _context.Personeller.Find(id);
            if (personel == null)
            {
                TempData["ErrorMessage"] = $"ID: {id} ile eşleşen personel bulunamadı.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Personeller.Remove(personel);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Personel başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Silme işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }

}