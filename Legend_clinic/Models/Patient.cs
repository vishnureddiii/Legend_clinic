using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("Patient")]
public partial class Patient
{
    [Key]
    [Column("PatientID")]
    public int PatientId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("DOB")]
    public DateOnly Dob { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Gender { get; set; } = null!;

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

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
