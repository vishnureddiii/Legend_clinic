using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

namespace Legend_clinic.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // REGISTER (PATIENT ONLY)
        // =========================
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check duplicate username
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists!";
                return View(model);
            }

            // 1. Create Patient FIRST
            var patient = new Patient
            {
                Name = model.Name ?? model.UserName,
                Dob = model.Dob,
                Gender = model.Gender ?? "Unknown",
                Address = model.Address ?? "",
                Phone = model.Phone ?? "",
                Email = model.Email ?? "",
                Summary = ""
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // 2. Create User linked to Patient
            var user = new User
            {
                UserName = model.UserName,
                Password = model.Password, // (Later: hash this)
                Role = "Patient",
                ReferenceId = patient.PatientId,
                IsApproved = false // 🔥 Admin must approve
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registered successfully! Wait for admin approval.";
            return RedirectToAction("Login");
        }

        // =========================
        // LOGIN
        // =========================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserName == model.UserName &&
                    u.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password!";
                return View(model);
            }

            // 🔥 BLOCK UNAPPROVED USERS
            if (!user.IsApproved)
            {
                ViewBag.Error = "Your account is waiting for admin approval.";
                return View(model);
            }

            // SESSION
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("ReferenceId", user.ReferenceId);

            // REDIRECT BY ROLE
            return user.Role switch
            {
                "Admin" => RedirectToAction("AdminDashboard", "Home"),
                "Doctor" => RedirectToAction("DoctorDashboard", "Home"),
                "Patient" => RedirectToAction("PatientDashboard", "Home"),
                "Chemist" => RedirectToAction("ChemistDashboard", "Home"),
                "Supplier" => RedirectToAction("SupplierDashboard", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // =========================
        // LOGOUT
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}