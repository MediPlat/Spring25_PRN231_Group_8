using System.Linq.Expressions;
using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;

namespace MediPlat.Service.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<SubscriptionResponse> GetAllSubscriptions()
        {
            var subscriptions = _unitOfWork.Subscriptions.GetAll();
            return subscriptions.Select(sub => _mapper.Map<SubscriptionResponse>(sub)).AsQueryable();
        }

        public async Task<SubscriptionResponse> GetSubscriptionByIdAsync(Guid id)
        {
            var subscription = await _unitOfWork.Subscriptions.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found.");
            
            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task<SubscriptionResponse> AddSubscriptionAsync(SubscriptionRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var subscription = _mapper.Map<Subscription>(request);
                subscription.Id = Guid.NewGuid();
                subscription.UpdateDate = DateTime.Now;

                _unitOfWork.Subscriptions.Add(subscription);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<SubscriptionResponse>(subscription);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<SubscriptionResponse> UpdateSubscriptionAsync(Guid id, SubscriptionRequest request)
        {
            var subscription = await _unitOfWork.Subscriptions.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found.");

            _mapper.Map(request, subscription);
            subscription.UpdateDate = DateTime.Now;

            _unitOfWork.Subscriptions.UpdatePartial(subscription,
                s => s.Name,
                s => s.Price,
                s => s.Description,
                s => s.CreatedDate,
                s => s.UpdateDate);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SubscriptionResponse>(subscription);
        }

        public async Task DeleteSubscriptionAsync(Guid id)
        {
            var subscription = await _unitOfWork.Subscriptions.GetIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found.");
            bool hasDoctorSubscriptions = _unitOfWork.DoctorSubscriptions.GetList(ds => ds.SubscriptionId == id).Any();
            if (hasDoctorSubscriptions)
                throw new InvalidOperationException("Cannot delete this subscription as it is referenced by doctor subscriptions.");
            _unitOfWork.Subscriptions.Remove(subscription);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
