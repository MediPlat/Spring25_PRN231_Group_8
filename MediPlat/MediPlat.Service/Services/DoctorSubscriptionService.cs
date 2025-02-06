using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Repository.Repositories;
using MediPlat.Service.IServices;

namespace MediPlat.Service.Services
{
    public class DoctorSubscriptionService : IDoctorSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions(Guid doctorId)
        {
            var doctorSubscriptions = _unitOfWork.DoctorSubscriptions.GetList(ds => ds.DoctorId == doctorId);
            return doctorSubscriptions.Select(ds => _mapper.Map<DoctorSubscriptionResponse>(ds)).AsQueryable();
        }

        public async Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _unitOfWork.DoctorSubscriptions.GetAsync(ds => ds.Id == id && ds.DoctorId == doctorId);
            if (doctorSubscription == null)
                throw new KeyNotFoundException("Doctor subscription not found.");

            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request, Guid doctorId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
            doctorSubscription.Id = Guid.NewGuid();
            doctorSubscription.DoctorId = doctorId;
            doctorSubscription.UpdateDate = DateTime.Now;

            _unitOfWork.DoctorSubscriptions.Add(doctorSubscription);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscriptionRequest request, Guid doctorId)
        {
            var existingSubscription = await _unitOfWork.DoctorSubscriptions.GetIdAsync(id);
            if (existingSubscription == null || existingSubscription.DoctorId != doctorId)
            {
                throw new KeyNotFoundException($"Doctor subscription with ID {id} not found.");
            }

            existingSubscription.EnableSlot = request.EnableSlot;
            existingSubscription.UpdateDate = DateTime.Now;

            _unitOfWork.DoctorSubscriptions.Update(existingSubscription);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DoctorSubscriptionResponse>(existingSubscription);
        }

        public async Task DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _unitOfWork.DoctorSubscriptions.GetAsync(ds => ds.Id == id && ds.DoctorId == doctorId);
            if (doctorSubscription == null)
                throw new KeyNotFoundException("Doctor subscription not found.");
            var relatedData = _unitOfWork.Subscriptions.GetList(s => s.Id == doctorSubscription.SubscriptionId);
            if (relatedData.Any())
                throw new InvalidOperationException("Cannot delete this subscription as it is referenced in other data.");
            _unitOfWork.DoctorSubscriptions.Remove(doctorSubscription);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
