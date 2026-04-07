using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("PurchaseOrderHeader")]
public partial class PurchaseOrderHeader
{
    [Key]
    [Column("POID")]
    public int Poid { get; set; }

    [Column("ChemistID")]
    public int ChemistId { get; set; }

    [Column("SupplierID")]
    public int SupplierId { get; set; }

    [Column("PODate", TypeName = "datetime")]
    public DateTime Podate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [ForeignKey("ChemistId")]
    [InverseProperty("PurchaseOrderHeaders")]
    public virtual Chemist Chemist { get; set; } = null!;

    [InverseProperty("Po")]
    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    [ForeignKey("SupplierId")]
    [InverseProperty("PurchaseOrderHeaders")]
    public virtual Supplier Supplier { get; set; } = null!;
}
