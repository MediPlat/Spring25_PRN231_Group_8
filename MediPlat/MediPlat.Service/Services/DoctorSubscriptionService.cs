using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;

namespace MediPlat.Service.Services
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

        public IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions(Guid doctorId)
        {
            var doctorSubscriptions = _doctorSubscriptionRepository.GetList(ds => ds.DoctorId == doctorId);
            return doctorSubscriptions.Select(ds => _mapper.Map<DoctorSubscriptionResponse>(ds)).AsQueryable();
        }

        public async Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _doctorSubscriptionRepository.GetAsync(ds => ds.Id == id && ds.DoctorId == doctorId);
            if (doctorSubscription == null)
                throw new KeyNotFoundException("Doctor subscription not found.");

            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request, Guid doctorId)
        {
            var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
            doctorSubscription.Id = Guid.NewGuid();
            doctorSubscription.DoctorId = doctorId;
            doctorSubscription.UpdateDate = DateTime.UtcNow;

            _doctorSubscriptionRepository.Add(doctorSubscription);
            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscription doctorSubscription, Guid doctorId)
        {
            var existingSubscription = await _doctorSubscriptionRepository.GetIdAsync(id);
            if (existingSubscription == null || existingSubscription.DoctorId != doctorId)
            {
                throw new KeyNotFoundException($"Doctor subscription with ID {id} not found.");
            }

            existingSubscription.EnableSlot = doctorSubscription.EnableSlot;
            existingSubscription.UpdateDate = DateTime.UtcNow;

            _doctorSubscriptionRepository.Update(existingSubscription);
            return _mapper.Map<DoctorSubscriptionResponse>(existingSubscription);
        }

        public async Task DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _doctorSubscriptionRepository.GetAsync(ds => ds.Id == id && ds.DoctorId == doctorId);
            if (doctorSubscription == null)
                throw new KeyNotFoundException("Doctor subscription not found.");

            _doctorSubscriptionRepository.Remove(doctorSubscription);
        }
    }
}
