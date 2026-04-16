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

        // ================= DASHBOARD =================
        public IActionResult Index()
        {
            return View();
        }

        // ================= PURCHASE ORDERS =================

        public IActionResult Orders()
        {
            int? supplierId = HttpContext.Session.GetInt32("SupplierId");

            if (supplierId == null)
                return RedirectToAction("Login", "Account");

            var orders = _context.PurchaseOrderHeaders
                .Where(o => o.SupplierId == supplierId)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.PurchaseOrderHeaders
                .Include(o => o.PurchaseOrderDetails)
                .ThenInclude(d => d.Drug)
                .FirstOrDefault(o => o.Poid == id);

            return View(order);
        }

        public IActionResult Accept(int id)
        {
            var order = _context.PurchaseOrderHeaders.Find(id);

            if (order != null && order.Status == "Pending")
            {
                order.Status = "Accepted";
                _context.SaveChanges();
            }

            return RedirectToAction("Orders");
        }

        public IActionResult Reject(int id)
        {
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