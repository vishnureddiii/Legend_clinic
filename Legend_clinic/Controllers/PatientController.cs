using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

namespace Legend_clinic.Controllers
{
    public class PatientController : Controller
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

       
        private bool IsPatient()
        {
            return HttpContext.Session.GetString("Role") == "Patient";
        }

        
        public IActionResult Index()
        {
            if (!IsPatient())
                return RedirectToAction("AccessDenied", "Home");

            return View();
        }

      
        public IActionResult BookAppointment()
        {
            if (!IsPatient())
                return RedirectToAction("AccessDenied", "Home");

            
            ViewBag.Doctors = _context.Physicians.ToList();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment model)
        {
            try
            {
                var userName = HttpContext.Session.GetString("UserName");

                if (string.IsNullOrEmpty(userName))
                    return Content("Session expired");

                var patientId = _context.Users
                    .Where(u => u.UserName == userName && u.Role == "Patient")
                    .Select(u => u.ReferenceId)
                    .FirstOrDefault();

                if (patientId == 0)
                    return Content("Invalid patient mapping");

                if (!_context.Patients.Any(p => p.PatientId == patientId))
                    return Content("Patient does not exist in Patient table");

                model.PatientId = patientId;

                model.ScheduleStatus = "Pending";
                model.Criticality ??= "Low";
                model.Reason ??= "N/A";
                model.Note ??= "N/A";

                _context.Appointments.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyAppointments");
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }
        }

       
        public IActionResult MyAppointments()
        {
            if (!IsPatient())
                return RedirectToAction("AccessDenied", "Home");

            int patientId = HttpContext.Session.GetInt32("ReferenceId") ?? 0;

            var appointments = _context.Appointments
                .Include(a => a.Physician)
                .Where(a => a.PatientId == patientId)
                .ToList();

            return View(appointments);
        }
    }
}