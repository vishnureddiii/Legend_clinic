using Microsoft.AspNetCore.Mvc;

namespace Legend_clinic.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
