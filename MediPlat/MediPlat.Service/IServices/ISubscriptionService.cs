using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.IServices
{
    public interface ISubscriptionService
    {
        IQueryable<SubscriptionResponse> GetAllSubscriptions();
        Task<SubscriptionResponse> GetSubscriptionByIdAsync(Guid id);
        Task<SubscriptionResponse> AddSubscriptionAsync(SubscriptionRequest request);
        Task<SubscriptionResponse> UpdateSubscriptionAsync(Guid id, SubscriptionRequest request);
        Task DeleteSubscriptionAsync(Guid id);
    }
}
