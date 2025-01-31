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
    public class DoctorSubscriptionService : IDoctorSubscriptionService
    {
        private readonly IDoctorSubscriptionRepository _doctorSubscriptionRepository;
        private readonly IMapper _mapper;

        public DoctorSubscriptionService(IDoctorSubscriptionRepository doctorSubscriptionRepository, IMapper mapper)
        {
            _doctorSubscriptionRepository = doctorSubscriptionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorSubscriptionResponse>> GetAllDoctorSubscriptionsAsync()
        {
            var doctorSubscriptions = await _doctorSubscriptionRepository.GetAllDoctorSubscriptionsAsync();
            return _mapper.Map<IEnumerable<DoctorSubscriptionResponse>>(doctorSubscriptions);
        }

        public async Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id)
        {
            var doctorSubscription = await _doctorSubscriptionRepository.GetDoctorSubscriptionByIdAsync(id);
            if (doctorSubscription == null)
                throw new KeyNotFoundException($"DoctorSubscription with ID {id} not found.");

            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request)
        {
            var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
            await _doctorSubscriptionRepository.AddDoctorSubscriptionAsync(doctorSubscription);
            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscriptionRequest request)
        {
            var existingSubscription = await _doctorSubscriptionRepository.GetDoctorSubscriptionByIdAsync(id);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"DoctorSubscription with ID {id} not found.");

            _mapper.Map(request, existingSubscription);
            await _doctorSubscriptionRepository.UpdateDoctorSubscriptionAsync(existingSubscription);

            return _mapper.Map<DoctorSubscriptionResponse>(existingSubscription);
        }

        public async Task DeleteDoctorSubscriptionAsync(Guid id)
        {
            var existingSubscription = await _doctorSubscriptionRepository.GetDoctorSubscriptionByIdAsync(id);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"DoctorSubscription with ID {id} not found.");

            await _doctorSubscriptionRepository.DeleteDoctorSubscriptionAsync(id);
        }
    }
}
