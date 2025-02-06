using System.Linq.Expressions;
using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IService;

namespace MediPlat.Service.Service
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
            return _mapper.ProjectTo<SubscriptionResponse>(_subscriptionRepository.GetAllAsQueryable());
        }

        public async Task<SubscriptionResponse> GetSubscriptionByIdAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");

            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> AddSubscriptionAsync(SubscriptionRequest request)
        {
            var subscription = _mapper.Map<Subscription>(request);
            await _subscriptionRepository.AddAsync(subscription);
            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> UpdateSubscriptionAsync(Guid id, SubscriptionRequest request)
        {
            var existingSubscription = await _subscriptionRepository.GetIdAsync(id);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");

            _mapper.Map(request, existingSubscription);
            await _subscriptionRepository.UpdateAsync(existingSubscription);

            return _mapper.Map<SubscriptionResponse>(existingSubscription);
        }

        public async Task DeleteSubscriptionAsync(Guid id)
        {
            await _subscriptionRepository.DeleteAsync(id);
        }
    }
}