using Microsoft.AspNetCore.Mvc;

namespace WebApplication7.Controllers
{
    public class KullaniciController : Controller
    {
        public IActionResult UserPanel()
        {
            return View();
        }
    }
}
