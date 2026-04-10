using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Legend_clinic.Models
{
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
#warning Move connection string to configuration in production
            => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MedPlus_Db;Trusted_Connection=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);

                entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Physician).WithMany(p => p.Appointments)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Chemist>(entity =>
            {
                entity.HasKey(e => e.ChemistId);
            });

            modelBuilder.Entity<Drug>(entity =>
            {
                entity.HasKey(e => e.DrugId);
            });

            modelBuilder.Entity<DrugRequest>(entity =>
            {
                entity.HasKey(e => e.DrugRequestId);

                entity.HasOne(d => d.Physician).WithMany(p => p.DrugRequests)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId);
            });

            modelBuilder.Entity<Physician>(entity =>
            {
                entity.HasKey(e => e.PhysicianId);
            });

            modelBuilder.Entity<PhysicianAdvice>(entity =>
            {
                entity.HasKey(e => e.PhysicianAdviceId);

                entity.HasOne(d => d.Schedule).WithMany(p => p.PhysicianAdvices)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PhysicianPrescription>(entity =>
            {
                entity.HasKey(e => e.PhysicianPrescripId);

                entity.HasOne(d => d.Drug).WithMany(p => p.PhysicianPrescriptions)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PhysicianAdvice).WithMany(p => p.PhysicianPrescriptions)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PurchaseOrderDetail>(entity =>
            {
                entity.HasKey(e => e.PodetailId);

                entity.HasOne(d => d.Drug).WithMany(p => p.PurchaseOrderDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Po).WithMany(p => p.PurchaseOrderDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PurchaseOrderHeader>(entity =>
            {
                entity.HasKey(e => e.Poid);

                entity.HasOne(d => d.Chemist).WithMany(p => p.PurchaseOrderHeaders)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrderHeaders)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId);

                entity.HasOne(d => d.Appointment).WithMany(p => p.Schedules)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
            });

           
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    UserName = "admin",
                    Password = "admin123",
                    Role = "Admin",
                    ReferenceId = 1
                }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}