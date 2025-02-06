using MediPlat.Model.Model;

namespace MediPlat.Repository.IRepositories
{
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        IQueryable<Subscription> GetAllSubscriptions();
        Task<Subscription?> GetSubscriptionByIdAsync(Guid id);
        Task AddSubscriptionAsync(Subscription subscription);
        Task UpdateSubscriptionAsync(Subscription subscription);
        Task DeleteSubscriptionAsync(Guid id);
    }
}