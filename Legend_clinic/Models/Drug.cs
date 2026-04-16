using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("Drug")]
public partial class Drug
{
    [Key]
    [Column("DrugID")]
    public int DrugId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Title { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    public DateOnly Expiry { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Dosage { get; set; } = null!;

    [InverseProperty("Drug")]
    public virtual ICollection<PhysicianPrescription> PhysicianPrescriptions { get; set; } = new List<PhysicianPrescription>();

    [InverseProperty("Drug")]
    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
}