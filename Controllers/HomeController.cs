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

        // Kullan�c� Giri�i
        [HttpPost]
        public IActionResult UserLogin()
        {
            HttpContext.Session.SetString("UserRole", "User"); // Kullan�c� rol�n� session'a kaydet
            return RedirectToAction("Index");
        }

        // Admin Giri�i
        [HttpPost]
        public IActionResult AdminLogin()
        {
            HttpContext.Session.SetString("UserRole", "Admin"); // Admin rol�n� session'a kaydet
            return RedirectToAction("Index");
        }

        public IActionResult AddPersonnel()
        {
            // Session'dan rol� kontrol et
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                // Yetkisiz kullan�c�y� engelle ve mesaj g�nder
                TempData["ErrorMessage"] = "Bu sayfaya eri�im yetkiniz yok.";
                return RedirectToAction("Index"); // Ana sayfaya y�nlendir
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