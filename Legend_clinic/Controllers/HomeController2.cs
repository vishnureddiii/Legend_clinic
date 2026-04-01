using Microsoft.AspNetCore.Mvc;

namespace Legend_clinic.Controllers
{
    public class HomeController2 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
