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

        // Dashboard
        public IActionResult Dashboard()
        {
            int? physicianId = HttpContext.Session.GetInt32("PhysicianId");

            if (physicianId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // View Appointments
        public async Task<IActionResult> ViewAppointments()
        {
            int? physicianId = HttpContext.Session.GetInt32("PhysicianId");

            if (physicianId == null)
                return RedirectToAction("Login", "Account");

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Schedules)
                .Where(a => a.PhysicianId == physicianId.Value)
                .ToListAsync();

            return View(appointments);
        }

        // Give Advice GET
        [HttpGet]
        public IActionResult GiveAdvice(int scheduleId)
        {
            ViewBag.ScheduleId = scheduleId;
            return View();
        }

        // Give Advice POST
        [HttpPost]
        public async Task<IActionResult> GiveAdvice(int scheduleId, string advice)
        {
            var physicianAdvice = new PhysicianAdvice
            {
                ScheduleId = scheduleId,
                Advice = advice
            };

            _context.PhysicianAdvices.Add(physicianAdvice);
            await _context.SaveChangesAsync();

            return RedirectToAction("GivePrescription", new
            {
                physicianAdviceId = physicianAdvice.PhysicianAdviceId
            });
        }

        // Give Prescription GET
        [HttpPost]
        public async Task<IActionResult> GivePrescription(
     int physicianAdviceId,
     List<int> drugIds,
     List<string> prescriptions)
        {
            // ✅ Basic validation
            if (drugIds == null || prescriptions == null)
                return BadRequest("Invalid prescription data.");

            if (drugIds.Count != prescriptions.Count)
                return BadRequest("Drug list and prescription list do not match.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                for (int i = 0; i < drugIds.Count; i++)
                {
                    var physicianPrescription = new PhysicianPrescription
                    {
                        PhysicianAdviceId = physicianAdviceId,
                        DrugId = drugIds[i],
                        Prescription = prescriptions[i]
                    };

                    _context.PhysicianPrescriptions.Add(physicianPrescription);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("ViewAppointments");
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Something went wrong while saving prescriptions.");
            }
        }

        // Drug Request GET
        [HttpGet]
        public IActionResult DrugRequest()
        {
            return View();
        }

        // Drug Request POST
        [HttpPost]
        public async Task<IActionResult> DrugRequest(string drugInfoText)
        {
            int? physicianId = HttpContext.Session.GetInt32("PhysicianId");

            if (physicianId == null)
                return RedirectToAction("Login", "Account");

            var request = new DrugRequest
            {
                PhysicianId = physicianId.Value,
                DrugInfoText = drugInfoText,
                RequestDate = DateTime.Now,
                RequestStatus = "Pending"
            };

            _context.DrugRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    }
}