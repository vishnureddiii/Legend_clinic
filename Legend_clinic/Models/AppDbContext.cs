using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Chemist> Chemists { get; set; }

    public virtual DbSet<Drug> Drugs { get; set; }

    public virtual DbSet<DrugRequest> DrugRequests { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianAdvice> PhysicianAdvices { get; set; }

    public virtual DbSet<PhysicianPrescription> PhysicianPrescriptions { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MedPlus_Db;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA26F7A4B1D");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__571DF1D5");

            entity.HasOne(d => d.Physician).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Physi__5812160E");
        });

        modelBuilder.Entity<Chemist>(entity =>
        {
            entity.HasKey(e => e.ChemistId).HasName("PK__Chemist__C0D5B7B45EFB390A");
        });

        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.DrugId).HasName("PK__Drug__908D66F6F96DCF36");
        });

        modelBuilder.Entity<DrugRequest>(entity =>
        {
            entity.HasKey(e => e.DrugRequestId).HasName("PK__DrugRequ__AEE9D6502702E69C");

            entity.HasOne(d => d.Physician).WithMany(p => p.DrugRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DrugReque__Physi__656C112C");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC346C9A8F048");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("PK__Physicia__DFF5ED73542AE353");
        });

        modelBuilder.Entity<PhysicianAdvice>(entity =>
        {
            entity.HasKey(e => e.PhysicianAdviceId).HasName("PK__Physicia__82C62610147CA5A4");

            entity.HasOne(d => d.Schedule).WithMany(p => p.PhysicianAdvices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Physician__Sched__5EBF139D");
        });

        modelBuilder.Entity<PhysicianPrescription>(entity =>
        {
            entity.HasKey(e => e.PhysicianPrescripId).HasName("PK__Physicia__DC5A55206D0FC4C6");

            entity.HasOne(d => d.Drug).WithMany(p => p.PhysicianPrescriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Physician__DrugI__628FA481");

            entity.HasOne(d => d.PhysicianAdvice).WithMany(p => p.PhysicianPrescriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Physician__Physi__619B8048");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.PodetailId).HasName("PK__Purchase__4EB47B5E31F20C89");

            entity.HasOne(d => d.Drug).WithMany(p => p.PurchaseOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__DrugI__6D0D32F4");

            entity.HasOne(d => d.Po).WithMany(p => p.PurchaseOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseOr__POID__6C190EBB");
        });

        modelBuilder.Entity<PurchaseOrderHeader>(entity =>
        {
            entity.HasKey(e => e.Poid).HasName("PK__Purchase__5F02A2F48573B7E0");

            entity.HasOne(d => d.Chemist).WithMany(p => p.PurchaseOrderHeaders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Chemi__68487DD7");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrderHeaders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Suppl__693CA210");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69EA8421FD");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedule__Appoin__5BE2A6F2");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE6669470C5E895");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACB87ED573");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
