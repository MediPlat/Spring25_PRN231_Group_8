using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.IServices
{
    public interface IDoctorSubscriptionService
    {
        IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions(Guid doctorId);
        Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id, Guid doctorId);
        Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request, Guid doctorId);
        Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscription doctorSubscription, Guid doctorId);
        Task DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId);
    }
}
