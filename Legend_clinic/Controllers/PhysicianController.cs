using Legend_clinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Controllers
{
    public class PhysicianController : Controller
    {
        private readonly AppDbContext _context;

        public PhysicianController(AppDbContext context)
        {
            _context = context;
        }

        // ================= COMMON CHECK =================
        private bool IsDoctorLoggedIn()
        {
            return HttpContext.Session.GetString("Role") == "Doctor"
                && HttpContext.Session.GetInt32("ReferenceId") != null;
        }

        private int? GetDoctorId()
        {
            return HttpContext.Session.GetInt32("ReferenceId");
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ================= APPOINTMENTS =================
        public async Task<IActionResult> ViewAppointments()
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            var doctorId = GetDoctorId();

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Schedules)
                .Where(a => a.PhysicianId == doctorId)
                .ToListAsync();

            return View(appointments);
        }

        // ================= GIVE ADVICE =================
        [HttpGet]
        public IActionResult GiveAdvice(int scheduleId)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.ScheduleId = scheduleId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GiveAdvice(int scheduleId, string advice)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            var entity = new PhysicianAdvice
            {
                ScheduleId = scheduleId,
                Advice = advice
            };

            _context.PhysicianAdvices.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("GivePrescription", new
            {
                physicianAdviceId = entity.PhysicianAdviceId
            });
        }

        // ================= PRESCRIPTION =================
        [HttpGet]
        public IActionResult GivePrescription(int physicianAdviceId)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.PhysicianAdviceId = physicianAdviceId;
            ViewBag.Drugs = _context.Drugs.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GivePrescription(
            int physicianAdviceId,
            List<int> drugIds,
            List<string> prescriptions)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            for (int i = 0; i < drugIds.Count; i++)
            {
                _context.PhysicianPrescriptions.Add(new PhysicianPrescription
                {
                    PhysicianAdviceId = physicianAdviceId,
                    DrugId = drugIds[i],
                    Prescription = prescriptions[i]
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ViewAppointments");
        }

        // ================= DRUG REQUEST =================
        [HttpGet]
        public IActionResult DrugRequest()
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DrugRequest(string drugInfoText)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            var doctorId = GetDoctorId();

            _context.DrugRequests.Add(new DrugRequest
            {
                PhysicianId = doctorId.Value,
                DrugInfoText = drugInfoText,
                RequestDate = DateTime.Now,
                RequestStatus = "Pending"
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    }
}