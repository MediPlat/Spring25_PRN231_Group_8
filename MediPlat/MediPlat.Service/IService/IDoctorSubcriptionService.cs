using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediPlat.Service.IService
{
    public interface IDoctorSubscriptionService
    {
        Task<IEnumerable<DoctorSubscriptionResponse>> GetAllDoctorSubscriptionsAsync();
        Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id);
        Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request);
        Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscriptionRequest request);
        Task DeleteDoctorSubscriptionAsync(Guid id);
    }
}
