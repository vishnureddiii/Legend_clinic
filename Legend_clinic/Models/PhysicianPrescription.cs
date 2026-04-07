using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("PhysicianPrescription")]
public partial class PhysicianPrescription
{
    [Key]
    [Column("PhysicianPrescripID")]
    public int PhysicianPrescripId { get; set; }

    [Column("PhysicianAdviceID")]
    public int PhysicianAdviceId { get; set; }

    [Column("DrugID")]
    public int DrugId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Prescription { get; set; } = null!;

    [ForeignKey("DrugId")]
    [InverseProperty("PhysicianPrescriptions")]
    public virtual Drug Drug { get; set; } = null!;

    [ForeignKey("PhysicianAdviceId")]
    [InverseProperty("PhysicianPrescriptions")]
    public virtual PhysicianAdvice PhysicianAdvice { get; set; } = null!;
}
