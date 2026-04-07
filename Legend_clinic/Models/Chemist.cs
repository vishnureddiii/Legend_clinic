using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("Chemist")]
public partial class Chemist
{
    [Key]
    [Column("ChemistID")]
    public int ChemistId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Summary { get; set; } = null!;

    [InverseProperty("Chemist")]
    public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; } = new List<PurchaseOrderHeader>();
}
