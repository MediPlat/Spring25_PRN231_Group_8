using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Uid=sa;Pwd=123456;Database=MediPlat;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC27D28D50C8");

            entity.ToTable("AppointmentSlot");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Profile).WithMany(p => p.AppointmentSlots)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("FK__Appointme__Profi__6A30C649");

            entity.HasOne(d => d.Slot).WithMany(p => p.AppointmentSlots)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Appointme__SlotI__693CA210");
        });

        modelBuilder.Entity<AppointmentSlotMedicine>(entity =>
        {
            entity.HasKey(e => e.AppointmentSlotMedicineId).HasName("PK__Appointm__36E2BCBF664F5D53");

            entity.ToTable("AppointmentSlotMedicine");

            entity.Property(e => e.AppointmentSlotMedicineId)
                .ValueGeneratedNever()
                .HasColumnName("AppointmentSlotMedicineID");
            entity.Property(e => e.AppointmentSlotId).HasColumnName("AppointmentSlotID");
            entity.Property(e => e.Dosage).HasMaxLength(255);
            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");

            entity.HasOne(d => d.AppointmentSlot).WithMany(p => p.AppointmentSlotMedicines)
                .HasForeignKey(d => d.AppointmentSlotId)
                .HasConstraintName("FK__Appointme__Appoi__6FE99F9F");

            entity.HasOne(d => d.Medicine).WithMany(p => p.AppointmentSlotMedicines)
                .HasForeignKey(d => d.MedicineId)
                .HasConstraintName("FK__Appointme__Medic__70DDC3D8");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctor__3214EC27C1BD1DE2");

            entity.ToTable("Doctor");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AcademicTitle).HasMaxLength(255);
            entity.Property(e => e.AverageRating).HasColumnType("decimal(2, 1)");
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
            entity.HasKey(e => e.Id).HasName("PK__DoctorSu__3214EC2789746209");

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
                .HasConstraintName("FK__DoctorSub__Docto__656C112C");

            entity.HasOne(d => d.Subscription).WithMany(p => p.DoctorSubscriptions)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__DoctorSub__Subsc__6477ECF3");
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Experien__3214EC2738D8D77C");

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
                .HasConstraintName("FK__Experienc__Docto__6383C8BA");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Experiences)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Experienc__Speci__628FA481");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicine__3214EC27DB0EBCA5");

            entity.ToTable("Medicine");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.DosageForm).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.Strength).HasMaxLength(50);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patient__3214EC272ED763C3");

            entity.ToTable("Patient");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(255);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Profile__3214EC279238FB3E");

            entity.ToTable("Profile");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Sex).HasMaxLength(50);

            entity.HasOne(d => d.Patient).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Profile__Patient__619B8048");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC2743859363");

            entity.ToTable("Review");

            entity.HasIndex(e => e.SlotId, "UQ__Review__0A124A4E4A0D7AD0").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");

            entity.HasOne(d => d.Slot).WithOne(p => p.Review)
                .HasForeignKey<Review>(d => d.SlotId)
                .HasConstraintName("FK__Review__SlotID__6EF57B66");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC2737CADB96");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Services)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Services__Specia__66603565");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slot__3214EC2752749AD3");

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
                .HasConstraintName("FK__Slot__DoctorID__6754599E");

            entity.HasOne(d => d.Service).WithMany(p => p.Slots)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Slot__ServiceID__68487DD7");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Specialt__3214EC27E8D6B4BE");

            entity.ToTable("Specialty");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC27E19AAB71");

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
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC27A005D199");

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
                .HasConstraintName("FK__Transacti__Appoi__6B24EA82");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Transacti__Docto__6E01572D");

            entity.HasOne(d => d.Patient).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Transacti__Patie__6D0D32F4");

            entity.HasOne(d => d.Sub).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SubId)
                .HasConstraintName("FK__Transacti__SubID__6C190EBB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
