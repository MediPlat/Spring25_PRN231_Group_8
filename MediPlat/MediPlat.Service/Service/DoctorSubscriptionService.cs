using MediPlat.Model;
using MediPlat.Repository.IRepositories;
using MediPlat.Repository.Repositories;
using MediPlat.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.Service
{
    public class DoctorSubscriptionService : IDoctorSubscriptionService
    {
        private readonly IDoctorSubscriptionRepository _doctorSubscriptionRepository;

        public DoctorSubscriptionService(IDoctorSubscriptionRepository doctorSubscriptionRepository)
        {
            _doctorSubscriptionRepository = doctorSubscriptionRepository;
        }

        public async Task<IEnumerable<DoctorSubscription>> GetAllDoctorSubscriptionsAsync()
        {
            return await _doctorSubscriptionRepository.GetAllDoctorSubscriptionsAsync();
        }

        public async Task<DoctorSubscription> GetDoctorSubscriptionByIdAsync(Guid id)
        {
            return await _doctorSubscriptionRepository.GetDoctorSubscriptionByIdAsync(id);
        }

        public async Task AddDoctorSubscriptionAsync(DoctorSubscription doctorSubscription)
        {
            await _doctorSubscriptionRepository.AddDoctorSubscriptionAsync(doctorSubscription);
        }

        public async Task UpdateDoctorSubscriptionAsync(DoctorSubscription doctorSubscription)
        {
            await _doctorSubscriptionRepository.UpdateDoctorSubscriptionAsync(doctorSubscription);
        }

        public async Task DeleteDoctorSubscriptionAsync(Guid id)
        {
            await _doctorSubscriptionRepository.DeleteDoctorSubscriptionAsync(id);
        }
    }
}
