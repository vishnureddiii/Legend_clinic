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

        // ================= GIVE ADVICE + MULTIPLE PRESCRIPTIONS =================
        [HttpGet]
        public IActionResult GiveAdvice(int appointmentId)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            if (appointmentId <= 0)
                return RedirectToAction("ViewAppointments");

            ViewBag.AppointmentId = appointmentId;

            // ✅ Ensure drugs always loaded
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

            // ✅ ALWAYS reload dropdown data
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

            // ✅ Get or create schedule
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

            // ✅ Save Advice
            var physicianAdvice = new PhysicianAdvice
            {
                ScheduleId = schedule.ScheduleId,
                Advice = advice
            };

            _context.PhysicianAdvices.Add(physicianAdvice);
            await _context.SaveChangesAsync();

            // ✅ Save MULTIPLE prescriptions safely
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

        // ================= EXTRA PRESCRIPTION =================
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GivePrescription(
            int physicianAdviceId,
            List<int> drugIds,
            List<string> prescriptions)
        {
            if (!IsDoctorLoggedIn())
                return RedirectToAction("Login", "Account");

            if (drugIds != null && prescriptions != null)
            {
                for (int i = 0; i < drugIds.Count; i++)
                {
                    if (i < prescriptions.Count &&
                        !string.IsNullOrWhiteSpace(prescriptions[i]))
                    {
                        _context.PhysicianPrescriptions.Add(new PhysicianPrescription
                        {
                            PhysicianAdviceId = physicianAdviceId,
                            DrugId = drugIds[i],
                            Prescription = prescriptions[i]
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Prescription saved successfully.";

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

            // ✅ EXTRA SAFETY (avoid null crash)
            if (doctorId == null)
                return RedirectToAction("Login", "Account");

            _context.DrugRequests.Add(new DrugRequest
            {
                PhysicianId = doctorId.Value,
                DrugInfoText = drugInfoText,
                RequestDate = DateTime.Now,
                RequestStatus = "Pending"
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Drug request sent to chemist successfully.";

            return RedirectToAction("DrugRequest");
        }
    }
}