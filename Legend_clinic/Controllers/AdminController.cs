using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

namespace Legend_clinic.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

      
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

     
        public IActionResult CreateUser()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                int referenceId = 0;

                
                if (model.Role == "Doctor")
                {
                    var doctor = new Physician
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        Specialization = model.Specialization ?? "",
                        Summary = model.Summary ?? ""
                    };
                    _context.Physicians.Add(doctor);
                    await _context.SaveChangesAsync();
                    referenceId = doctor.PhysicianId;
                }
               
                else if (model.Role == "Chemist")
                {
                    var chemist = new Chemist
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        Summary = model.Summary ?? ""
                    };
                    _context.Chemists.Add(chemist);
                    await _context.SaveChangesAsync();
                    referenceId = chemist.ChemistId;
                }
                
                else if (model.Role == "Supplier")
                {
                    var supplier = new Supplier
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email
                    };
                    _context.Suppliers.Add(supplier);
                    await _context.SaveChangesAsync();
                    referenceId = supplier.SupplierId;
                }

               
                var user = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Role = model.Role,
                    ReferenceId = referenceId,
                    IsApproved = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminDashboard", "Home");
            }

            return View(model);
        }

      
        public async Task<IActionResult> PendingPatients()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var users = await _context.Users
                .Where(u => u.Role == "Patient" && !u.IsApproved)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Approve(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsApproved = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingPatients");
        }

        public async Task<IActionResult> Reject(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingPatients");
        }

        
        public IActionResult ManagePhysicians()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var doctors = _context.Physicians.ToList();
            return View(doctors);
        }

        public IActionResult EditPhysician(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var doctor = _context.Physicians.Find(id);
            if (doctor == null) return NotFound();

            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> EditPhysician(Physician model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                _context.Physicians.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManagePhysicians");
            }

            return View(model);
        }

        public async Task<IActionResult> DeletePhysician(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var doctor = await _context.Physicians.FindAsync(id);
            if (doctor != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.ReferenceId == id && u.Role == "Doctor");
                if (user != null) _context.Users.Remove(user);

                _context.Physicians.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManagePhysicians");
        }

        
        public IActionResult ManageChemists()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var chemists = _context.Chemists.ToList();
            return View(chemists);
        }

        public IActionResult EditChemist(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var chemist = _context.Chemists.Find(id);
            if (chemist == null) return NotFound();
            return View(chemist);
        }

        [HttpPost]
        public async Task<IActionResult> EditChemist(Chemist model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                _context.Chemists.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageChemists");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteChemist(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.ReferenceId == id && u.Role == "Chemist");
                if (user != null) _context.Users.Remove(user);

                _context.Chemists.Remove(chemist);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageChemists");
        }

        
        public IActionResult ManageSuppliers()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var suppliers = _context.Suppliers.ToList();
            return View(suppliers);
        }

        public IActionResult EditSupplier(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var supplier = _context.Suppliers.Find(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> EditSupplier(Supplier model)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                _context.Suppliers.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageSuppliers");
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteSupplier(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.ReferenceId == id && u.Role == "Supplier");
                if (user != null) _context.Users.Remove(user);

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageSuppliers");
        }
       
        public async Task<IActionResult> PendingAppointments()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var list = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Physician)
                .Where(a => a.ScheduleStatus == "Pending")
                .ToListAsync();

            return View(list);
        }
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var appt = await _context.Appointments.FindAsync(id);

            if (appt != null)
            {
                appt.ScheduleStatus = "Approved";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingAppointments");
        }
        public async Task<IActionResult> RejectAppointment(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");

            var appt = await _context.Appointments.FindAsync(id);

            if (appt != null)
            {
                appt.ScheduleStatus = "Rejected";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingAppointments");
        }
    }

}

