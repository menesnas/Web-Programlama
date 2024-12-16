using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Kullanýcý Giriþi
        [HttpPost]
        public IActionResult UserLogin()
        {
            HttpContext.Session.SetString("UserRole", "User"); // Kullanýcý rolünü session'a kaydet
            return RedirectToAction("Index");
        }

        // Admin Giriþi
        [HttpPost]
        public IActionResult AdminLogin()
        {
            HttpContext.Session.SetString("UserRole", "Admin"); // Admin rolünü session'a kaydet
            return RedirectToAction("Index");
        }

        public IActionResult AddPersonnel()
        {
            // Session'dan rolü kontrol et
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                // Yetkisiz kullanýcýyý engelle ve mesaj gönder
                TempData["ErrorMessage"] = "Bu sayfaya eriþim yetkiniz yok.";
                return RedirectToAction("Index"); // Ana sayfaya yönlendir
            }

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}