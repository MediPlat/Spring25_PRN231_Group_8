using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.Extensions.Logging;

namespace MediPlat.Service.Services
{
    public class DoctorSubscriptionService : IDoctorSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorSubscriptionService> _logger;
        public DoctorSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DoctorSubscriptionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions()
        {
            return _unitOfWork.DoctorSubscriptions
                .GetAll(ds => ds.Doctor, ds => ds.Subscription)
                .Select(ds => _mapper.Map<DoctorSubscriptionResponse>(ds)).AsQueryable();
        }

        public async Task<DoctorSubscriptionResponse> GetDoctorSubscriptionByIdAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _unitOfWork.DoctorSubscriptions.GetAsync(
                ds => ds.Id == id && ds.DoctorId == doctorId,
                ds => ds.Doctor,
                ds => ds.Subscription
            );

            if (doctorSubscription == null)
            {
                throw new KeyNotFoundException("Doctor subscription not found.");
            }

            return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
        }

        public async Task<DoctorSubscriptionResponse> AddDoctorSubscriptionAsync(DoctorSubscriptionRequest request, Guid doctorId)
        {
            var existingSubscription = await _unitOfWork.DoctorSubscriptions
                .GetAsync(ds => ds.DoctorId == doctorId && ds.EndDate >= DateTime.Now);

            if (existingSubscription != null)
            {
                throw new InvalidOperationException("Doctor đã có một Subscription đang hoạt động. Không thể tạo mới.");
            }

            var subscription = await _unitOfWork.Subscriptions.GetAsync(s => s.Id == request.SubscriptionId);
            if (subscription == null)
            {
                throw new KeyNotFoundException("Subscription không tồn tại.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
                doctorSubscription.Id = Guid.NewGuid();
                doctorSubscription.DoctorId = doctorId;
                doctorSubscription.EnableSlot = subscription.EnableSlot;
                doctorSubscription.StartDate = now;
                doctorSubscription.EndDate = now.AddMonths(1);
                doctorSubscription.Status = "Active";
                doctorSubscription.UpdateDate = null;

                _unitOfWork.DoctorSubscriptions.Add(doctorSubscription);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<DoctorSubscriptionResponse>(doctorSubscription);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<DoctorSubscriptionResponse> UpdateDoctorSubscriptionAsync(Guid id, DoctorSubscriptionRequest request, Guid doctorId)
        {
            var existingSubscription = await _unitOfWork.DoctorSubscriptions.GetIdAsync(id);
            if (existingSubscription == null || existingSubscription.DoctorId != doctorId)
            {
                throw new KeyNotFoundException($"Doctor subscription with ID {id} not found.");
            }

            if (request.EnableSlot.HasValue && request.EnableSlot.Value != existingSubscription.EnableSlot)
            {
                existingSubscription.EnableSlot = request.EnableSlot.Value;
            }
            else
            {
                _logger.LogWarning("EnableSlot remains unchanged. Skipping update.");
            }

            existingSubscription.UpdateDate = DateTime.Now;

            await _unitOfWork.DoctorSubscriptions.UpdatePartialAsync(existingSubscription,
                ds => ds.EnableSlot,
                ds => ds.UpdateDate);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("After Update: Subscription {Id} - Updated EnableSlot: {EnableSlot}",
                id, existingSubscription.EnableSlot);

            return _mapper.Map<DoctorSubscriptionResponse>(existingSubscription);
        }

        public async Task<bool> DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId)
        {
            var doctorSubscription = await _unitOfWork.DoctorSubscriptions.GetAsync(ds => ds.Id == id && ds.DoctorId == doctorId);
            if (doctorSubscription == null)
            {
                throw new KeyNotFoundException("Doctor subscription not found or you don't have permission to delete it.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.DoctorSubscriptions.Remove(doctorSubscription);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

    }
}
