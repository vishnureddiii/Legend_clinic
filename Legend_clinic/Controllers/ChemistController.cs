using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

namespace Legend_clinic.Controllers
{
    public class ChemistController : Controller
    {
        private readonly AppDbContext _context;

        public ChemistController(AppDbContext context)
        {
            _context = context;
        }

        // ================= DASHBOARD COUNTS =================
        private async Task LoadDashboardCounts()
        {
            ViewBag.TotalDrugs = await _context.Drugs.CountAsync();
            ViewBag.TotalRequests = await _context.DrugRequests.CountAsync();
            ViewBag.TotalOrders = await _context.PurchaseOrderHeaders.CountAsync();
        }

        // ================= DASHBOARD =================
        public async Task<IActionResult> Index()
        {
            await LoadDashboardCounts();
            return View();
        }

        // ================= DRUGS =================
        public async Task<IActionResult> ManageDrugs()
        {
            await LoadDashboardCounts();

            var drugs = await _context.Drugs
                .OrderByDescending(d => d.DrugId)
                .ToListAsync();

            return View(drugs);
        }

        // ================= ADD DRUG =================
        public async Task<IActionResult> AddDrug()
        {
            await LoadDashboardCounts();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDrug(Drug drug)
        {
            await LoadDashboardCounts();

            // ✅ FIX: Expiry validation (DateOnly safe)
            if (drug.Expiry < new DateOnly(2026, 1, 1))
            {
                ModelState.AddModelError("Expiry", "Expiry date cannot be before 2026.");
            }

            if (!ModelState.IsValid)
                return View(drug);

            _context.Drugs.Add(drug);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Drug added successfully!";
            return RedirectToAction(nameof(ManageDrugs));
        }

        // ================= CREATE PURCHASE ORDER =================
        public async Task<IActionResult> CreatePurchaseOrder()
        {
            await LoadDashboardCounts();

            var model = new CreatePOViewModel
            {
                Suppliers = await _context.Suppliers
                    .Select(s => new SelectListItem
                    {
                        Value = s.SupplierId.ToString(),
                        Text = s.Name
                    }).ToListAsync(),

                Drugs = await _context.Drugs
                    .Select(d => new SelectListItem
                    {
                        Value = d.DrugId.ToString(),
                        Text = d.Title
                    }).ToListAsync(),

                Items = new List<POItem> { new POItem() }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchaseOrder(CreatePOViewModel model)
        {
            await LoadDashboardCounts();

            // reload dropdowns
            model.Suppliers = await _context.Suppliers
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.Name
                }).ToListAsync();

            model.Drugs = await _context.Drugs
                .Select(d => new SelectListItem
                {
                    Value = d.DrugId.ToString(),
                    Text = d.Title
                }).ToListAsync();

            // ================= VALIDATION =================
            if (model.SupplierId <= 0)
                ModelState.AddModelError("SupplierId", "Please select supplier.");

            if (model.Items == null || !model.Items.Any())
                ModelState.AddModelError("", "Please add at least one item.");

            foreach (var item in model.Items ?? new List<POItem>())
            {
                if (item.DrugId <= 0)
                    ModelState.AddModelError("", "Please select drug.");

                // ✅ FIX: Quantity validation (NO negative / zero)
                if (item.Quantity <= 0)
                    ModelState.AddModelError("", "Quantity must be greater than 0.");
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var chemist = await _context.Chemists.FirstOrDefaultAsync();

                if (chemist == null)
                {
                    ModelState.AddModelError("", "Chemist not found.");
                    return View(model);
                }

                // ================= HEADER =================
                var header = new PurchaseOrderHeader
                {
                    ChemistId = chemist.ChemistId,
                    SupplierId = model.SupplierId,
                    Podate = DateTime.Now,
                    Status = "Pending"
                };

                _context.PurchaseOrderHeaders.Add(header);
                await _context.SaveChangesAsync();

                // ================= DETAILS =================
                foreach (var item in model.Items)
                {
                    if (item.DrugId > 0 && item.Quantity > 0)
                    {
                        _context.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                        {
                            Poid = header.Poid,
                            DrugId = item.DrugId,
                            Quantity = item.Quantity
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Purchase Order created successfully!";
                return RedirectToAction(nameof(CreatePurchaseOrder));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // ================= DRUG REQUESTS =================
        public async Task<IActionResult> DrugRequests()
        {
            await LoadDashboardCounts();

            var data = await _context.DrugRequests
                .Include(x => x.Physician)
                .OrderByDescending(x => x.DrugRequestId)
                .ToListAsync();

            return View(data);
        }

        // ================= UPDATE REQUEST =================
        public async Task<IActionResult> UpdateRequestStatus(int id, string status)
        {
            var req = await _context.DrugRequests.FindAsync(id);

            if (req == null)
                return NotFound();

            req.RequestStatus = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DrugRequests));
        }
    }
}