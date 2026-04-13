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

     
        public async Task<IActionResult> Index()
        {
            var drugs = await _context.Drugs.ToListAsync();
            return View(drugs);
        }
    }
}