using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("Appointment")]
public partial class Appointment
{
    [Key]
    [Column("AppointmentID")]
    public int AppointmentId { get; set; }

    [Column("PatientID")]
    public int  PatientId { get; set; }

    [Column("PhysicianID")]
    public int ? PhysicianId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime AppointmentDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Criticality { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Reason { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Note { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string ScheduleStatus { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("PhysicianId")]
    [InverseProperty("Appointments")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
