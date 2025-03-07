using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.IServices
{
    public interface IDoctorSubscriptionService
    {
        IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions();
        Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id, Guid doctorId);
        Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request, Guid doctorId);
        Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscriptionRequest request, Guid doctorId);
        Task<bool> DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId);
    }
}
