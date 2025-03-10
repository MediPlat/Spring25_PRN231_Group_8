using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediPlat.Service.Services
{
    public class MediPlatService : IMediPlatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MediPlatService> _logger;

        public MediPlatService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MediPlatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse> AddServiceAsync(ServiceRequest request)
        {
            try
            {
                _logger.LogInformation("Adding new service");

                // Map từ request sang entity
                var service = _mapper.Map<MediPlat.Model.Model.Service>(request);
                service.Id = Guid.NewGuid(); // Tạo mới Id

                // Thêm vào database
                await _unitOfWork.Services.AddAsync(service);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Service added successfully with ID: {service.Id}");
                return _mapper.Map<ServiceResponse>(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding service");
                throw;
            }
        }

        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete service with ID: {id}");

                var service = await _unitOfWork.Services.GetAsync(s => s.Id == id);
                if (service == null)
                {
                    _logger.LogWarning($"Service with ID: {id} not found");
                    return false;
                }

                _unitOfWork.Services.Remove(service);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Service with ID: {id} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting service with ID: {id}");
                throw;
            }
        }

        public IQueryable<ServiceResponse> GetAllServices()
        {
            try
            {
                return _unitOfWork.Services.GetAll(s => s.Specialty).ToList().Select(se => _mapper.Map<ServiceResponse>(se)).AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all services");
                throw;
            }
        }

        public async Task<ServiceResponse> GetServiceByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Retrieving service with ID: {id}");

                var service = await _unitOfWork.Services.GetAsync(s => s.Id == id);
                if (service == null)
                {
                    _logger.LogWarning($"Service with ID: {id} not found");
                    return null;
                }

                return _mapper.Map<ServiceResponse>(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving service with ID: {id}");
                throw;
            }
        }

        public async Task<ServiceResponse> UpdateServiceAsync(Guid id, ServiceRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating service with ID: {id}");

                var existingService = await _unitOfWork.Services.GetAsync(s => s.Id == id);
                if (existingService == null)
                {
                    _logger.LogWarning($"Service with ID: {id} not found");
                    throw new Exception("Service not found");
                }

                // Map request vào entity hiện có
                _mapper.Map(request, existingService);

                // Cập nhật trong database
                _unitOfWork.Services.Update(existingService);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Service with ID: {id} updated successfully");
                return _mapper.Map<ServiceResponse>(existingService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating service with ID: {id}");
                throw;
            }
        }
    }
}