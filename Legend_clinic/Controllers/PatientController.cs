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

                var user = _context.Users
                    .FirstOrDefault(u => u.UserName == userName && u.Role == "Patient");

                if (user == null)
                    return Content("User not found");

                if (user.ReferenceId == null || user.ReferenceId == 0)
                    return Content("Patient mapping missing in Users table");

                var patientId = user.ReferenceId;

                var patientExists = _context.Patients
                    .Any(p => p.PatientId == patientId);

                if (!patientExists)
                    return Content("Patient does not exist in Patient table");

                // ✅ FIXED PART
                model.PatientId = patientId;

                // IMPORTANT: doctor assigned later
                model.PhysicianId = null;   // ONLY works if int?

                model.ScheduleStatus = "Pending";
                model.Criticality = string.IsNullOrEmpty(model.Criticality) ? "Low" : model.Criticality;
                model.Reason = string.IsNullOrEmpty(model.Reason) ? "N/A" : model.Reason;
                model.Note = string.IsNullOrEmpty(model.Note) ? "N/A" : model.Note;

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
        public IActionResult MyPrescriptions()
        {
            if (!IsPatient())
                return RedirectToAction("AccessDenied", "Home");

            int patientId = HttpContext.Session.GetInt32("ReferenceId") ?? 0;

            var prescriptions = _context.PhysicianPrescriptions
    .Include(p => p.Drug)
    .Include(p => p.PhysicianAdvice)
        .ThenInclude(a => a.Schedule)
            .ThenInclude(s => s.Appointment)
    .Where(p => p.PhysicianAdvice.Schedule.Appointment.PatientId == patientId)
    .ToList();

            return View(prescriptions);
        }

    }
}