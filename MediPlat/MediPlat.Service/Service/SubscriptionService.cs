using AutoMapper;
using MediPlat.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<SubscriptionResponse>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync();
            return _mapper.Map<IEnumerable<SubscriptionResponse>>(subscriptions);
        }

        public async Task<SubscriptionResponse> GetSubscriptionByIdAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");

            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> AddSubscriptionAsync(SubscriptionRequest request)
        {
            var subscription = _mapper.Map<Subscription>(request);
            subscription.Id = Guid.NewGuid(); // Ensure ID is generated
            await _subscriptionRepository.AddSubscriptionAsync(subscription);

            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> UpdateSubscriptionAsync(Guid id, SubscriptionRequest request)
        {
            var existingSubscription = await _subscriptionRepository.GetSubscriptionByIdAsync(id);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");

            _mapper.Map(request, existingSubscription);
            await _subscriptionRepository.UpdateSubscriptionAsync(existingSubscription);

            return _mapper.Map<SubscriptionResponse>(existingSubscription);
        }

        public async Task DeleteSubscriptionAsync(Guid id)
        {
            var existingSubscription = await _subscriptionRepository.GetSubscriptionByIdAsync(id);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"Subscription with ID {id} not found.");

            await _subscriptionRepository.DeleteSubscriptionAsync(id);
        }
    }
}
