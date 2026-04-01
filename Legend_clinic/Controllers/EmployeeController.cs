using Microsoft.AspNetCore.Mvc;

namespace Legend_clinic.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
