using System;
using System.Collections.Generic;
using MediPlat.Model.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MediPlat.Model.Model;

public partial class MediPlatContext : DbContext
{
    public MediPlatContext()
    {
    }

    public MediPlatContext(DbContextOptions<MediPlatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppointmentSlot> AppointmentSlots { get; set; }

    public virtual DbSet<AppointmentSlotMedicine> AppointmentSlotMedicines { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSubscription> DoctorSubscriptions { get; set; }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Retrieve the connection string from appsettings.json
            var connectionString = GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    private string GetConnectionString()
    {
        // Build the configuration to read from appsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        return configuration.GetConnectionString("DB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC2754C500ED");

            entity.ToTable("AppointmentSlot");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Patient).WithMany(p => p.AppointmentSlots)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Appointme__Patie__6383C8BA");

            entity.HasOne(d => d.Slot).WithMany(p => p.AppointmentSlots)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Appointme__SlotI__628FA481");
        });

        modelBuilder.Entity<AppointmentSlotMedicine>(entity =>
        {
            entity.HasKey(e => e.AppointmentSlotMedicineId).HasName("PK__Appointm__36E2BCBF99310D02");

            entity.ToTable("AppointmentSlotMedicine");

            entity.Property(e => e.AppointmentSlotMedicineId)
                .ValueGeneratedNever()
                .HasColumnName("AppointmentSlotMedicineID");
            entity.Property(e => e.AppointmentSlotId).HasColumnName("AppointmentSlotID");
            entity.Property(e => e.Dosage).HasMaxLength(255);
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");

            entity.HasOne(d => d.AppointmentSlot).WithMany(p => p.AppointmentSlotMedicines)
                .HasForeignKey(d => d.AppointmentSlotId)
                .HasConstraintName("FK__Appointme__Appoi__6B24EA82");

            entity.HasOne(d => d.Medicine).WithMany(p => p.AppointmentSlotMedicines)
                .HasForeignKey(d => d.MedicineId)
                .HasConstraintName("FK__Appointme__Medic__6C190EBB");

            entity.HasOne(d => d.Patient).WithMany(p => p.AppointmentSlotMedicines)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Appointme__Patie__6D0D32F4");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctor__3214EC27B4AE83AE");

            entity.ToTable("Doctor");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AcademicTitle).HasMaxLength(255);
            entity.Property(e => e.Balance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Degree).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FeePerHour).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(255);
        });

        modelBuilder.Entity<DoctorSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DoctorSu__3214EC272A08B916");

            entity.ToTable("DoctorSubscription");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSubscriptions)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__DoctorSub__Docto__5EBF139D");

            entity.HasOne(d => d.Subscription).WithMany(p => p.DoctorSubscriptions)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__DoctorSub__Subsc__5DCAEF64");
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Experien__3214EC27E75614EC");

            entity.ToTable("Experience");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Experienc__Docto__5CD6CB2B");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Experienc__Speci__5BE2A6F2");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicine__3214EC2750D62237");

            entity.ToTable("Medicine");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DosageForm).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Strength).HasMaxLength(50);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patient__3214EC2790ABF7E6");

            entity.ToTable("Patient");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Sex).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(255);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC272AB51463");

            entity.ToTable("Review");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Review__DoctorID__693CA210");

            entity.HasOne(d => d.Patient).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Review__PatientI__68487DD7");

            entity.HasOne(d => d.Slot).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Review__SlotID__6A30C649");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC2739BF248C");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Services)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Services__Specia__5FB337D6");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slot__3214EC271B8C7892");

            entity.ToTable("Slot");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SessionFee).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Slots)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Slot__DoctorID__60A75C0F");

            entity.HasOne(d => d.Service).WithMany(p => p.Slots)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Slot__ServiceID__619B8048");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Specialt__3214EC27A6AC7AF2");

            entity.ToTable("Specialty");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC27E5D0A299");

            entity.ToTable("Subscription");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC272C8B3D2A");

            entity.ToTable("Transaction");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AppointmentSlotId).HasColumnName("AppointmentSlotID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubId).HasColumnName("SubID");

            entity.HasOne(d => d.AppointmentSlot).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AppointmentSlotId)
                .HasConstraintName("FK__Transacti__Appoi__6477ECF3");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Transacti__Docto__6754599E");

            entity.HasOne(d => d.Patient).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Transacti__Patie__66603565");

            entity.HasOne(d => d.Sub).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SubId)
                .HasConstraintName("FK__Transacti__SubID__656C112C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
