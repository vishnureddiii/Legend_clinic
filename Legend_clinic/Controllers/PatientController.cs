using Microsoft.AspNetCore.Mvc;

namespace Legend_clinic.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
