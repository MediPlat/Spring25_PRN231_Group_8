using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.IServices
{
    public interface IMediPlatService
    {
        IQueryable<ServiceResponse> GetAllServices();
        Task<ServiceResponse> GetServiceByIdAsync(Guid id);
        Task<ServiceResponse> AddServiceAsync(ServiceRequest request);
        Task<ServiceResponse> UpdateServiceAsync(Guid id, ServiceRequest request);
        Task<bool> DeleteServiceAsync(Guid id);
    }
}
