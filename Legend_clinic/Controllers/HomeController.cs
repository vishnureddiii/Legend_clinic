using Microsoft.AspNetCore.Mvc;

namespace Legend_clinic.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
