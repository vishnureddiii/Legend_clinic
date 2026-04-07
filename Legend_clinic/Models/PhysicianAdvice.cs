using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("PhysicianAdvice")]
public partial class PhysicianAdvice
{
    [Key]
    [Column("PhysicianAdviceID")]
    public int PhysicianAdviceId { get; set; }

    [Column("ScheduleID")]
    public int ScheduleId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Advice { get; set; } = null!;

    [InverseProperty("PhysicianAdvice")]
    public virtual ICollection<PhysicianPrescription> PhysicianPrescriptions { get; set; } = new List<PhysicianPrescription>();

    [ForeignKey("ScheduleId")]
    [InverseProperty("PhysicianAdvices")]
    public virtual Schedule Schedule { get; set; } = null!;
}
