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

        // Orders List (ONLY logged-in supplier)
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

        // Order Details
        public IActionResult Details(int id)
        {
            var order = _context.PurchaseOrderHeaders
                .Include(o => o.PurchaseOrderDetails)
                .ThenInclude(d => d.Drug)
                .FirstOrDefault(o => o.Poid == id);

            return View(order);
        }

        // Accept Order
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

        // Reject Order
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

        // ================= DRUG REQUESTS =================

        // View Drug Requests (for supplier)
        public IActionResult DrugRequests()
        {
            int? supplierId = HttpContext.Session.GetInt32("SupplierId");

            //if (supplierId == null)
            //    return RedirectToAction("Login", "Account");

            // If SupplierId column exists in DrugRequest use it
            var requests = _context.DrugRequests
                .Include(r => r.Physician)
                //.Where(r => r.SupplierId == supplierId) // Uncomment if you add SupplierId
                .Where(r => r.RequestStatus == "Pending")
                .ToList();

            return View(requests);
        }

        // Drug Request Details (optional but useful)
        public IActionResult DrugRequestDetails(int id)
        {
            var request = _context.DrugRequests
                .Include(r => r.Physician)
                .FirstOrDefault(r => r.DrugRequestId == id);

            return View(request);
        }

        // Accept Drug Request
        public IActionResult AcceptDrugRequest(int id)
        {
            var request = _context.DrugRequests.Find(id);

            if (request != null && request.RequestStatus == "Pending")
            {
                request.RequestStatus = "Accepted";

                // OPTIONAL: convert to Purchase Order automatically
                /*
                var order = new PurchaseOrderHeader
                {
                    SupplierId = HttpContext.Session.GetInt32("SupplierId"),
                    OrderDate = DateTime.Now,
                    Status = "Pending"
                };

                _context.PurchaseOrderHeaders.Add(order);
                */

                _context.SaveChanges();
            }

            return RedirectToAction("DrugRequests");
        }

        // Reject Drug Request
        public IActionResult RejectDrugRequest(int id)
        {
            var request = _context.DrugRequests.Find(id);

            if (request != null && request.RequestStatus == "Pending")
            {
                request.RequestStatus = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("DrugRequests");
        }
    }
}