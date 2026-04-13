using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

public class ChemistController : Controller
{
    private readonly AppDbContext _context;

    public ChemistController(AppDbContext context)
    {
        _context = context;
    }

    // ================= DASHBOARD =================
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalDrugs = await _context.Drugs.CountAsync();
        ViewBag.TotalRequests = await _context.DrugRequests.CountAsync();
        return View();
    }

    // ================= DRUGS =================
    public async Task<IActionResult> ManageDrugs()
    {
        var drugs = await _context.Drugs.ToListAsync();
        return View(drugs);
    }

    public IActionResult AddDrug()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDrug(Drug drug)
    {
        if (!ModelState.IsValid)
        {
            return View(drug);
        }

        _context.Drugs.Add(drug);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ManageDrugs));
    }

    // ================= PURCHASE ORDER =================

    // GET
    public IActionResult CreatePurchaseOrder()
    {
        var model = new CreatePOViewModel
        {
            Suppliers = _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.SupplierId.ToString(),
                Text = s.Name
            }).ToList(),

            Drugs = _context.Drugs.Select(d => new SelectListItem
            {
                Value = d.DrugId.ToString(),
                Text = d.Title
            }).ToList()
        };

        return View(model);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePurchaseOrder(CreatePOViewModel model)
    {
        Console.WriteLine("🔥 POST HIT - CreatePurchaseOrder");

        if (!ModelState.IsValid)
        {
            // reload dropdowns again
            model.Suppliers = _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.SupplierId.ToString(),
                Text = s.Name
            }).ToList();

            model.Drugs = _context.Drugs.Select(d => new SelectListItem
            {
                Value = d.DrugId.ToString(),
                Text = d.Title
            }).ToList();

            return View(model);
        }

        var header = new PurchaseOrderHeader
        {
            ChemistId = 1,
            SupplierId = model.SupplierId,
            Podate = DateTime.Now,
            Status = "Pending"
        };

        _context.PurchaseOrderHeaders.Add(header);
        await _context.SaveChangesAsync();

        var detail = new PurchaseOrderDetail
        {
            Poid = header.Poid,
            DrugId = model.DrugId,
            Quantity = model.Quantity
        };

        _context.PurchaseOrderDetails.Add(detail);
        await _context.SaveChangesAsync();
        ViewBag.Message = "sufully submited purchase order";

        Console.WriteLine("✅ ORDER SAVED SUCCESSFULLY");
        TempData["Success"] = "Order created successfully!";
        return RedirectToAction(nameof(CreatePurchaseOrder));

       // return RedirectToAction(nameof(Index));
    }

    // ================= DRUG REQUESTS =================

    public async Task<IActionResult> DrugRequests()
    {
        var data = await _context.DrugRequests
            .Include(x => x.Physician)
            .ToListAsync();

        return View(data);
    }

    [HttpGet]
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