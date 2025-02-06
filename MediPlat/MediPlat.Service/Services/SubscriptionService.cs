using System.Linq.Expressions;
using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;

namespace MediPlat.Service.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
        }

        public IQueryable<SubscriptionResponse> GetAllSubscriptions()
        {
            var subscriptions = _subscriptionRepository.GetAll();
            return subscriptions.Select(sub => _mapper.Map<SubscriptionResponse>(sub)).AsQueryable();
        }

        public async Task<SubscriptionResponse> GetSubscriptionByIdAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found.");
            
            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> AddSubscriptionAsync(SubscriptionRequest request)
        {
            var subscription = _mapper.Map<Subscription>(request);
            subscription.Id = Guid.NewGuid();
            subscription.UpdateDate = DateTime.UtcNow;

            _subscriptionRepository.Add(subscription);
            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> UpdateSubscriptionAsync(Guid id, SubscriptionRequest request)
        {
            var subscription = await _subscriptionRepository.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found.");

            _mapper.Map(request, subscription);
            subscription.UpdateDate = DateTime.UtcNow;

            _subscriptionRepository.Update(subscription);
            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task DeleteSubscriptionAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found.");

            _subscriptionRepository.Remove(subscription);
        }
    }
}
