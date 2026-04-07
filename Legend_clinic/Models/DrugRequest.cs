using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("DrugRequest")]
public partial class DrugRequest
{
    [Key]
    [Column("DrugRequestID")]
    public int DrugRequestId { get; set; }

    [Column("PhysicianID")]
    public int PhysicianId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string DrugInfoText { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime RequestDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string RequestStatus { get; set; } = null!;

    [ForeignKey("PhysicianId")]
    [InverseProperty("DrugRequests")]
    public virtual Physician Physician { get; set; } = null!;
}
