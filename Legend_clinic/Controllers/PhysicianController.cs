using Legend_clinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Controllers
{
    public class PhysicianController : Controller
    {
        private readonly AppDbContext _context;

        // ✅ FIXED CONSTRUCTOR
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

        // ============================================================
        // ================= VIEW APPOINTMENTS ========================
        // ============================================================
        public async Task<IActionResult> ViewAppointments()
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            var doctorId = GetDoctorId();

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Schedules)
                    .ThenInclude(s => s.PhysicianAdvices)
                .Where(a => a.PhysicianId == doctorId)
                .ToListAsync();

            // ✅ NULL SAFE
            appointments = appointments ?? new List<Appointment>();

            // ✅ TODAY RANGE FIX (IMPORTANT)
            var start = DateTime.Today;
            var end = start.AddDays(1);

            // ================= TODAY =================
            ViewBag.TodayAppointments = appointments
                .Where(a => a.AppointmentDateTime >= start &&
                            a.AppointmentDateTime < end)
                .ToList();

            // ================= UPCOMING =================
            ViewBag.UpcomingAppointments = appointments
                .Where(a => a.AppointmentDateTime >= end)
                .ToList();

            // ================= COMPLETED =================
            ViewBag.CompletedAppointments = appointments
                .Where(a => a.Schedules != null &&
                            a.Schedules.Any(s => s.ScheduleStatus == "Completed"))
                .ToList();

            return View(appointments);
        }

        // ============================================================
        // ================= GIVE ADVICE ==============================
        // ============================================================
        [HttpGet]
        public IActionResult GiveAdvice(int appointmentId)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.AppointmentId = appointmentId;
            ViewBag.Drugs = _context.Drugs.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveAdvice(
            int appointmentId,
            string advice,
            List<int> drugIds,
            List<string> prescriptions)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.AppointmentId = appointmentId;
            ViewBag.Drugs = _context.Drugs.ToList();

            if (string.IsNullOrWhiteSpace(advice))
            {
                ViewBag.Error = "Advice is required.";
                return View();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                ViewBag.Error = "Invalid appointment.";
                return View();
            }

            // ❗ Prevent duplicate
            var existingAdvice = await _context.PhysicianAdvices
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.Schedule.AppointmentId == appointmentId);

            if (existingAdvice != null)
            {
                TempData["Error"] = "Prescription already exists.";
                return RedirectToAction("ViewAppointments");
            }

            // ✅ Schedule
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);

            if (schedule == null)
            {
                schedule = new Schedule
                {
                    AppointmentId = appointmentId,
                    ScheduleDate = appointment.AppointmentDateTime,
                    ScheduleStatus = "Completed"
                };

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
            }

            // ✅ Advice
            var physicianAdvice = new PhysicianAdvice
            {
                ScheduleId = schedule.ScheduleId,
                Advice = advice
            };

            _context.PhysicianAdvices.Add(physicianAdvice);
            await _context.SaveChangesAsync();

            // ✅ Prescriptions
            if (drugIds != null && prescriptions != null)
            {
                for (int i = 0; i < drugIds.Count; i++)
                {
                    if (i < prescriptions.Count &&
                        !string.IsNullOrWhiteSpace(prescriptions[i]))
                    {
                        _context.PhysicianPrescriptions.Add(new PhysicianPrescription
                        {
                            PhysicianAdviceId = physicianAdvice.PhysicianAdviceId,
                            DrugId = drugIds[i],
                            Prescription = prescriptions[i]
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Advice and prescription sent successfully.";
            return RedirectToAction("ViewAppointments");
        }

        // ============================================================
        // ================= VIEW PRESCRIPTION ========================
        // ============================================================
        public async Task<IActionResult> ViewPrescription(int appointmentId)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            var data = await _context.PhysicianAdvices
                .Include(a => a.Schedule)
                .Include(a => a.PhysicianPrescriptions)
                    .ThenInclude(p => p.Drug)
                .Where(a => a.Schedule.AppointmentId == appointmentId)
                .ToListAsync();

            if (data == null || !data.Any())
            {
                TempData["Error"] = "No prescription found.";
                return RedirectToAction("ViewAppointments");
            }

            return View("ViewPrescription", data);
        }

        // ============================================================
        // ================= DRUG REQUEST =============================
        // ============================================================
        [HttpGet]
        public IActionResult DrugRequest()
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DrugRequest(string drugInfoText)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(drugInfoText))
            {
                TempData["Error"] = "Please enter drug details.";
                return RedirectToAction("DrugRequest");
            }

            var doctorId = GetDoctorId();

            _context.DrugRequests.Add(new DrugRequest
            {
                PhysicianId = doctorId.Value,
                DrugInfoText = drugInfoText,
                RequestDate = DateTime.Now,
                RequestStatus = "Pending"
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Drug request sent successfully.";
            return RedirectToAction("DrugRequest");
        }
    }
}