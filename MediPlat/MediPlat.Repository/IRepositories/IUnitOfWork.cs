using System;
using MediPlat.Model.Model;

namespace MediPlat.Repository.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<DoctorSubscription> DoctorSubscriptions { get; }
        IGenericRepository<Subscription> Subscriptions { get; }
        IGenericRepository<Patient> Patients { get; }
        IGenericRepository<Doctor> Doctors { get; }
        IGenericRepository<Experience> Experiences { get; }
        IGenericRepository<AppointmentSlotMedicine> AppointmentSlotMedicines { get; }
        IGenericRepository<AppointmentSlot> AppointmentSlots { get; }
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Medicine> Medicines { get; }
        IGenericRepository<AppointmentSlot> AppointmentsSlots { get; }
        IGenericRepository<Slot> Slots { get; }
        IGenericRepository<Specialty> Specialties { get; }
        IGenericRepository<Profile> Profiles { get; }
        IGenericRepository<Slot> Slots { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
