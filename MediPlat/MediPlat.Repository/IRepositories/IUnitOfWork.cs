using System;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using Microsoft.EntityFrameworkCore.Storage;

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
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Medicine> Medicines { get; }
        IGenericRepository<Specialty> Specialties { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
