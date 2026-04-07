using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("PurchaseOrderDetail")]
public partial class PurchaseOrderDetail
{
    [Key]
    [Column("PODetailID")]
    public int PodetailId { get; set; }

    [Column("POID")]
    public int Poid { get; set; }

    [Column("DrugID")]
    public int DrugId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("DrugId")]
    [InverseProperty("PurchaseOrderDetails")]
    public virtual Drug Drug { get; set; } = null!;

    [ForeignKey("Poid")]
    [InverseProperty("PurchaseOrderDetails")]
    public virtual PurchaseOrderHeader Po { get; set; } = null!;
}
