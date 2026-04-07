using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("Physician")]
public partial class Physician
{
    [Key]
    [Column("PhysicianID")]
    public int PhysicianId { get; set; }

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

    [StringLength(100)]
    [Unicode(false)]
    public string Specialization { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Summary { get; set; } = null!;

    [InverseProperty("Physician")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Physician")]
    public virtual ICollection<DrugRequest> DrugRequests { get; set; } = new List<DrugRequest>();
}
