using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MediPlat.Model;

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

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSubscription> DoctorSubscriptions { get; set; }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Uid=sa;Pwd=123456;Database=MediPlat; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC27456285F7");

            entity.ToTable("AppointmentSlot");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Patient).WithMany(p => p.AppointmentSlots)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Appointme__Patie__59063A47");

            entity.HasOne(d => d.Slot).WithMany(p => p.AppointmentSlots)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Appointme__SlotI__5812160E");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctor__3214EC275379655F");

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
            entity.Property(e => e.FeePerSession).HasColumnType("decimal(12, 2)");
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
            entity.HasKey(e => e.Id).HasName("PK__DoctorSu__3214EC2783F3045C");

            entity.ToTable("DoctorSubscription");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSubscriptions)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__DoctorSub__Docto__534D60F1");

            entity.HasOne(d => d.Subscription).WithMany(p => p.DoctorSubscriptions)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__DoctorSub__Subsc__52593CB8");
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Experien__3214EC279A6C4B16");

            entity.ToTable("Experience");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Experienc__Docto__5165187F");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Experienc__Speci__5070F446");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patient__3214EC273C7831E2");

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
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC27D9D395F6");

            entity.ToTable("Review");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Review__DoctorID__5EBF139D");

            entity.HasOne(d => d.Patient).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Review__PatientI__5DCAEF64");

            entity.HasOne(d => d.Slot).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Review__SlotID__5FB337D6");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC27A9A4611E");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Services)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Services__Specia__5441852A");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slot__3214EC277926FC3B");

            entity.ToTable("Slot");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.SessionFee).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Slots)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Slot__DoctorID__5535A963");

            entity.HasOne(d => d.Service).WithMany(p => p.Slots)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Slot__ServiceID__5629CD9C");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Slots)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Slot__SpecialtyI__571DF1D5");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Specialt__3214EC2702E195FD");

            entity.ToTable("Specialty");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC276B3E33CA");

            entity.ToTable("Subscription");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC27C8B91882");

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
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SubId).HasColumnName("SubID");

            entity.HasOne(d => d.AppointmentSlot).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AppointmentSlotId)
                .HasConstraintName("FK__Transacti__Appoi__59FA5E80");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Transacti__Docto__5CD6CB2B");

            entity.HasOne(d => d.Patient).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Transacti__Patie__5BE2A6F2");

            entity.HasOne(d => d.Sub).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SubId)
                .HasConstraintName("FK__Transacti__SubID__5AEE82B9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
