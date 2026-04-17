using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

namespace Legend_clinic.Controllers
{
    public class SupplierController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierController(AppDbContext context)
        {
            _context = context;
        }

        // ================= HELPER METHOD =================
        private bool IsSupplier(out int supplierId)
        {
            supplierId = HttpContext.Session.GetInt32("ReferenceId") ?? 0;
            var role = HttpContext.Session.GetString("Role");

            return role == "Supplier" && supplierId > 0;
        }

        // ================= DASHBOARD =================
        public IActionResult Index()
        {
            if (!IsSupplier(out _))
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ================= PURCHASE ORDERS =================
        public IActionResult Orders()
        {
            if (!IsSupplier(out int supplierId))
                return RedirectToAction("Login", "Account");

            var orders = _context.PurchaseOrderHeaders
                .Where(o => o.SupplierId == supplierId)
                .OrderByDescending(o => o.Poid)
                .ToList();

            return View(orders);
        }

        // ================= ORDER DETAILS =================
        public IActionResult Details(int id)
        {
            if (!IsSupplier(out _))
                return RedirectToAction("Login", "Account");

            var order = _context.PurchaseOrderHeaders
                .Include(o => o.PurchaseOrderDetails)
                .ThenInclude(d => d.Drug)
                .FirstOrDefault(o => o.Poid == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // ================= ACCEPT ORDER =================
        public IActionResult Accept(int id)
        {
            if (!IsSupplier(out _))
                return RedirectToAction("Login", "Account");

            var order = _context.PurchaseOrderHeaders.Find(id);

            if (order != null && order.Status == "Pending")
            {
                order.Status = "Accepted";
                _context.SaveChanges();
            }

            return RedirectToAction("Orders");
        }

        // ================= REJECT ORDER =================
        public IActionResult Reject(int id)
        {
            if (!IsSupplier(out _))
                return RedirectToAction("Login", "Account");

            var order = _context.PurchaseOrderHeaders.Find(id);

            if (order != null && order.Status == "Pending")
            {
                order.Status = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("Orders");
        }
    }
}