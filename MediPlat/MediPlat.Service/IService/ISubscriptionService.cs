using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediPlat.Service.IService
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionResponse>> GetAllSubscriptionsAsync();
        Task<SubscriptionResponse> GetSubscriptionByIdAsync(Guid id);
        Task<SubscriptionResponse> AddSubscriptionAsync(SubscriptionRequest request);
        Task<SubscriptionResponse> UpdateSubscriptionAsync(Guid id, SubscriptionRequest request);
        Task DeleteSubscriptionAsync(Guid id);
    }
}
