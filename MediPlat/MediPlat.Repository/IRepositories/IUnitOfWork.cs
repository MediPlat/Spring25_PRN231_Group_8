using System;
using System.Threading.Tasks;
using MediPlat.Model.Model;

namespace MediPlat.Repository.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<DoctorSubscription> DoctorSubscriptions { get; }
        IGenericRepository<Subscription> Subscriptions { get; }
        IGenericRepository<Patient> Patients { get; }

        Task<int> SaveChangesAsync();
    }
}
