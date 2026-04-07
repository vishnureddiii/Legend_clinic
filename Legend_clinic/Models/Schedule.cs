using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

[Table("Schedule")]
public partial class Schedule
{
    [Key]
    [Column("ScheduleID")]
    public int ScheduleId { get; set; }

    [Column("AppointmentID")]
    public int AppointmentId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ScheduleDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ScheduleStatus { get; set; } = null!;

    [ForeignKey("AppointmentId")]
    [InverseProperty("Schedules")]
    public virtual Appointment Appointment { get; set; } = null!;

    [InverseProperty("Schedule")]
    public virtual ICollection<PhysicianAdvice> PhysicianAdvices { get; set; } = new List<PhysicianAdvice>();
}
