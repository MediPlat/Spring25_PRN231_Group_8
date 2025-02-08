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
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var now = DateTime.Now;

                var existingSubscription = _unitOfWork.DoctorSubscriptions
                    .GetList(ds => ds.DoctorId == doctorId)
                    .OrderByDescending(ds => ds.EndDate)
                    .FirstOrDefault();

                if (existingSubscription != null && existingSubscription.EndDate > now)
                {
                    var newSubscription = await _unitOfWork.Subscriptions.GetIdAsync(request.SubscriptionId);
                    if (!existingSubscription.SubscriptionId.HasValue)
                    {
                        throw new InvalidOperationException("Existing subscription has no valid SubscriptionId.");
                    }

                    var oldSubscription = await _unitOfWork.Subscriptions.GetIdAsync(existingSubscription.SubscriptionId.Value);

                    if (newSubscription.Price <= oldSubscription.Price)
                    {
                        throw new InvalidOperationException("You can only upgrade to a higher subscription.");
                    }
                }

                var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
                doctorSubscription.Id = Guid.NewGuid();
                doctorSubscription.DoctorId = doctorId;
                doctorSubscription.StartDate = now;
                doctorSubscription.EndDate = now.AddMonths(1);
                doctorSubscription.UpdateDate = now;

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

            existingSubscription.EnableSlot = request.EnableSlot;
            existingSubscription.UpdateDate = DateTime.Now;

            await _unitOfWork.DoctorSubscriptions.UpdatePartialAsync(existingSubscription,
                ds => ds.EnableSlot,
                ds => ds.UpdateDate);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DoctorSubscriptionResponse>(existingSubscription);
        }

        public Task DeleteDoctorSubscriptionAsync(Guid id, Guid doctorId)
        {
            throw new InvalidOperationException("Deleting a DoctorSubscription is not allowed.");
        }
    }
}
