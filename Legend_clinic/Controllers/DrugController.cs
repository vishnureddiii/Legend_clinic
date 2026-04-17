using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Legend_clinic.Models;

namespace Legend_clinic.Controllers
{
    public class DrugController : Controller
    {
        private readonly AppDbContext _context;

        public DrugController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Drug/Index?search=xxx
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Drugs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.Title.ToLower().Contains(search.ToLower())
                );
            }

            var drugs = await query.ToListAsync();
            return View(drugs);
        }

        // GET: /Drug/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var drug = await _context.Drugs
                .FirstOrDefaultAsync(x => x.DrugId == id);

            if (drug == null)
            {
                return NotFound();
            }

            return View(drug);
        }
    }
}