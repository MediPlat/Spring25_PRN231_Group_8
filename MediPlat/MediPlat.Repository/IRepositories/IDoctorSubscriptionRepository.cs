using MediPlat.Model.Model;

namespace MediPlat.Repository.IRepositories
{
    public interface IDoctorSubscriptionRepository : IGenericRepository<DoctorSubscription>
    {
        IQueryable<DoctorSubscription> GetAllDoctorSubscriptions(Guid doctorId);
        Task<DoctorSubscription?> GetDoctorSubscriptionByIdAsync(Guid id);
        Task AddDoctorSubscriptionAsync(DoctorSubscription doctorSubscription);
        Task UpdateDoctorSubscriptionAsync(DoctorSubscription doctorSubscription);
        Task DeleteDoctorSubscriptionAsync(Guid id);
    }
}