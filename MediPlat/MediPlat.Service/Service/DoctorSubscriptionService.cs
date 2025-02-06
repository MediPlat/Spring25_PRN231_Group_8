using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IService;

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

        public IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions(Guid doctorId)
        {
            return _mapper.ProjectTo<DoctorSubscriptionResponse>(_doctorSubscriptionRepository.GetAllDoctorSubscriptions(doctorId));
        }


        public async Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _doctorSubscriptionRepository.GetIdAsync(id);
            if (doctorSubscription == null || doctorSubscription.DoctorId != doctorId)
                throw new KeyNotFoundException("DoctorSubscription not found or unauthorized.");

            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request, Guid doctorId)
        {
            var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
            doctorSubscription.DoctorId = doctorId;
            await _doctorSubscriptionRepository.AddAsync(doctorSubscription);
            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscriptionRequest request, Guid doctorId)
        {
            var existingDoctorSubscription = await _doctorSubscriptionRepository.GetIdAsync(id);
            if (existingDoctorSubscription == null || existingDoctorSubscription.DoctorId != doctorId)
                throw new KeyNotFoundException("DoctorSubscription not found or unauthorized.");

            _mapper.Map(request, existingDoctorSubscription);
            await _doctorSubscriptionRepository.UpdateAsync(existingDoctorSubscription);

            return _mapper.Map<DoctorSubscriptionResponse>(existingDoctorSubscription);
        }

        public async Task DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _doctorSubscriptionRepository.GetIdAsync(id);
            if (doctorSubscription == null || doctorSubscription.DoctorId != doctorId)
                throw new KeyNotFoundException("DoctorSubscription not found or unauthorized.");

            await _doctorSubscriptionRepository.DeleteAsync(id);
        }
    }
}

