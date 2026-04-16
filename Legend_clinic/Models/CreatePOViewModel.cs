using Microsoft.AspNetCore.Mvc.Rendering;

namespace Legend_clinic.Models
{
    public class CreatePOViewModel
    {
        public int SupplierId { get; set; }

        // ✅ MULTIPLE ITEMS
        public List<POItem> Items { get; set; } = new List<POItem>();

        public List<SelectListItem>? Suppliers { get; set; }
        public List<SelectListItem>? Drugs { get; set; }
    }

    public class POItem
    {
        public int DrugId { get; set; }
        public int Quantity { get; set; }
    }
}