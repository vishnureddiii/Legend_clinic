using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Legend_clinic.Controllers
{
    public class HomeController : Controller
    {
        
        private bool CheckRole(string role)
        {
            return HttpContext.Session.GetString("Role") == role;
        }

       
        public IActionResult Index()
        {
            return View();
        }

       
        public IActionResult AdminDashboard()
        {
            if (!CheckRole("Admin"))
                return RedirectToAction("AccessDenied");

            return View();
        }

        
        public IActionResult DoctorDashboard()
        {
            if (!CheckRole("Doctor"))
                return RedirectToAction("AccessDenied");

            return View();
        }

        
        public IActionResult PatientDashboard()
        {
            if (!CheckRole("Patient"))
                return RedirectToAction("AccessDenied");

            return View();
        }

        
        public IActionResult ChemistDashboard()
        {
            if (!CheckRole("Chemist"))
                return RedirectToAction("AccessDenied");

            return View();
        }

        
        public IActionResult SupplierDashboard()
        {
            if (!CheckRole("Supplier"))
                return RedirectToAction("AccessDenied");

            return View();
        }

        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}