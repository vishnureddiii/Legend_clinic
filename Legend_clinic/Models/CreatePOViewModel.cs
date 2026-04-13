using Microsoft.AspNetCore.Mvc.Rendering;

namespace Legend_clinic.Models
{
    public class CreatePOViewModel
    {
        public int SupplierId { get; set; }
        public int DrugId { get; set; }
        public int Quantity { get; set; }

        public List<SelectListItem>? Suppliers { get; set; }
        public List<SelectListItem>? Drugs { get; set; }
    }
}