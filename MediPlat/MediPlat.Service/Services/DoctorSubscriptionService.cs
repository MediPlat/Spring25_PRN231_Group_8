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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<DoctorSubscriptionResponse> GetAllDoctorSubscriptions(Guid doctorId)
        {
            return _unitOfWork.DoctorSubscriptions
                .GetList(ds => ds.DoctorId == doctorId)
                .AsQueryable()
                .Select(ds => _mapper.Map<DoctorSubscriptionResponse>(ds));
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
                var doctorSubscription = _mapper.Map<DoctorSubscription>(request);
                doctorSubscription.Id = Guid.NewGuid();
                doctorSubscription.DoctorId = doctorId;
                doctorSubscription.StartDate = now;
                doctorSubscription.EndDate = now.AddMonths(1);
                doctorSubscription.Status = "Đang hoạt động";
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
            existingSubscription.Status = request.Status;

            await _unitOfWork.DoctorSubscriptions.UpdatePartialAsync(existingSubscription,
                ds => ds.EnableSlot,
                ds => ds.UpdateDate,
                ds => ds.Status);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DoctorSubscriptionResponse>(existingSubscription);
        }
    }
}
