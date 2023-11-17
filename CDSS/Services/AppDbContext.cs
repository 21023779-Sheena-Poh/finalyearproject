using System;
using System.Collections.Generic;
using CDSS.Models;
using Microsoft.EntityFrameworkCore;

namespace CDSS.Services;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointments> Appointments { get; set; }

    public virtual DbSet<MedicalStaff> MedicalStaff { get; set; }

    public virtual DbSet<Medication> Medication { get; set; }

    public virtual DbSet<PatientMedication> PatientMedication { get; set; }

    public virtual DbSet<Patients> Patients { get; set; }

    public virtual DbSet<Review> Review { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointments>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA23488D730");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AdditionalNotes).IsUnicode(false);
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.PurposeOfVisit).IsUnicode(false);

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__286302EC");
        });

        modelBuilder.Entity<MedicalStaff>(entity =>
        {
            entity.HasKey(e => e.MedicalStaffId).HasName("PK__MedicalS__7ED40F8954339AF7");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.Appointment).WithMany(p => p.MedicalStaff)
                .UsingEntity<Dictionary<string, object>>(
                    "MedicalStaffAppointments",
                    r => r.HasOne<Appointments>().WithMany()
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MedicalSt__Appoi__33D4B598"),
                    l => l.HasOne<MedicalStaff>().WithMany()
                        .HasForeignKey("MedicalStaffId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MedicalSt__Medic__32E0915F"),
                    j =>
                    {
                        j.HasKey("MedicalStaffId", "AppointmentId").HasName("PK__MedicalS__4638D0633AA708F4");
                        j.IndexerProperty<int>("MedicalStaffId").HasColumnName("MedicalStaffID");
                        j.IndexerProperty<int>("AppointmentId").HasColumnName("AppointmentID");
                    });
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.MedicationId).HasName("PK__Medicati__62EC1ADABF9F25F3");

            entity.Property(e => e.MedicationId).HasColumnName("MedicationID");
            entity.Property(e => e.Class)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MedicationName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PatientMedication>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Duration)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EndMedication).HasColumnType("datetime");
            entity.Property(e => e.Frequency)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MedicationId).HasColumnName("MedicationID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.StartMedication).HasColumnType("datetime");

            entity.HasOne(d => d.Medication).WithMany()
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PatientMe__Medic__300424B4");

            entity.HasOne(d => d.Patient).WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PatientMe__Patie__2F10007B");
        });

        modelBuilder.Entity<Patients>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346E221D24B");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Bed)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Birthdate).HasColumnType("date");
            entity.Property(e => e.BloodType)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MedicalCondition).IsUnicode(false);
            entity.Property(e => e.Ward)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79AE067F0A43");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ReviewDateTime).HasColumnType("datetime");
            entity.Property(e => e.ReviewText).IsUnicode(false);

            entity.HasOne(d => d.Appointment).WithMany(p => p.Review)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__Appointm__2B3F6F97");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
